using System.Windows;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions.Input;
using Ogur.Abstractions.Windows;
using Ogur.Capabilities.Fishing;
using Ogur.Fishing.Host.Wpf.Adapters;
using Ogur.Fishing.Host.Wpf.Services;
using Ogur.Fishing.Host.Wpf.Services.Models;
using BaitOption = Ogur.Fishing.Host.Wpf.Services.Models.BaitOption;
using ProcessOption = Ogur.Fishing.Host.Wpf.Services.Models.ProcessOption;

namespace Ogur.Fishing.Host.Wpf.ViewModels
{
    /// <summary>
    /// Main dashboard VM controlling services and the Fishing capability.
    /// </summary>
    public sealed partial class MainViewModel : ObservableObject
    {
        private readonly FishingCapability _fishing;
        private readonly IServiceProvider _sp;
        private readonly ILogger<MainViewModel> _logger;
        private readonly IBaitCatalog _baits;
        private readonly IProcessQueryService _processes;
        private readonly CancellationTokenSource _cts = new();
        private readonly ISessionState _session;
        private readonly IWindowActivator _activator;
        private readonly IKeyboardSynthesizer _keys;
        private readonly IFishingRunGate _runGate;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="fishing">Fishing capability.</param>
        /// <param name="sp">Service provider.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="baits">Bait catalog (slots).</param>
        /// <param name="processes">Process query service.</param>
        /// <param name="session">Session state.</param>
        /// <param name="keys">Keyboard synthesizer.</param>
        /// <param name="activator">Window activator.</param>
        public MainViewModel(
            FishingCapability fishing,
            IServiceProvider sp,
            ILogger<MainViewModel> logger,
            IBaitCatalog baits,
            IProcessQueryService processes,
            ISessionState session,
            IKeyboardSynthesizer keys,
            IFishingRunGate runGate,
            IWindowActivator activator)
        {
            _fishing = fishing;
            _sp = sp;
            _logger = logger;
            _baits = baits;
            _processes = processes;
            _session = session;
            _keys = keys;
            _runGate = runGate;
            _activator = activator;

            BaitItems = new ObservableCollection<BaitOption>(_baits.GetAll());
            Events = new ObservableCollection<string>();
            _ = ConsumeEventsAsync(_cts.Token);
            _ = RefreshProcessesAsync();
        }

        /// <summary>
        /// Gets or sets the current FSM-like status text.
        /// </summary>
        [ObservableProperty] private string _status = "Stopped";

        /// <summary>
        /// Gets the live log of capability events.
        /// </summary>
        public ObservableCollection<string> Events { get; }

        /// <summary>
        /// Gets the list of bait slot options.
        /// </summary>
        public ObservableCollection<BaitOption> BaitItems { get; }

        /// <summary>
        /// Gets or sets the selected bait slot.
        /// </summary>
        [ObservableProperty] private BaitOption? _selectedBait;

        /// <summary>
        /// Gets the candidate processes.
        /// </summary>
        public ObservableCollection<ProcessOption> Processes { get; } = new();

        /// <summary>
        /// Gets or sets the selected process.
        /// </summary>
        [ObservableProperty] private ProcessOption? _selectedProcess;

        /// <summary>
        /// Gets or sets the debug memory address text.
        /// </summary>
        [ObservableProperty] private string _memoryAddress = "0x00000000";

        /// <summary>
        /// Starts the fishing capability.
        /// </summary>
        /// <returns>Task.</returns>
        [RelayCommand]
        private async Task StartAsync()
        {
            if (_fishing.Status == Abstractions.CapabilityStatus.Running)
            {
                _logger.LogInformation("Start ignored: already running.");
                return;
            }
            
            try
            {
                if (_runGate is null || _fishing is null || _sp is null)
                {
                    _logger?.LogError("StartAsync: missing dependency (_runGate={RunGateNull}, _fishing={FishingNull}, _sp={SpNull})",
                        _runGate is null, _fishing is null, _sp is null);
                    await OnUiAsync(() => Events?.Add("Error: internal dependency not available."));
                    return;
                }

                // Nie wykonuj akcji zanim capability faktycznie wystartuje
                _runGate.Disable();

                int? slot = null;
                if (SelectedBait is not null)
                {
                    try { slot = MapWpfKeyToSlot(SelectedBait.Key); }
                    catch (Exception exMap)
                    {
                        _logger?.LogWarning(exMap, "StartAsync: MapWpfKeyToSlot failed; starting without slot.");
                        slot = null;
                    }
                }

                _fishing.ApplyUiSnapshot(slot);

                // UWAGA: bez ConfigureAwait(false) – kontynuacja wraca na kontekst UI
                await _fishing.StartAsync(new Abstractions.CapabilityStartContext(_sp), CancellationToken.None);

                // Dopiero po sukcesie startu odblokuj egzekucję akcji
                _runGate.Enable();

                await OnUiAsync(() => Status = _fishing.Status.ToString());
                _logger?.LogInformation("Fishing started with bait slot {Slot} on PID {Pid}", SelectedBait?.Id, SelectedProcess?.Pid);
            }
            catch (Exception ex)
            {
                // Capability mogła rzucić (w logach masz NRE w FishingCapability.cs:163) – nie zabijaj UI.
                _runGate?.Disable();
                _logger?.LogError(ex, "Failed to start fishing.");
                await OnUiAsync(() => Events?.Add($"Error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Stops the fishing capability.
        /// </summary>
        /// <returns>Task.</returns>
        [RelayCommand]
        private async Task StopAsync()
        {
            try
            {
                _runGate?.Disable(); // natychmiast odcinamy executor

                if (_fishing is null)
                {
                    _logger?.LogWarning("StopAsync: capability is null.");
                    await OnUiAsync(() => Events?.Add("Warning: capability not available."));
                    return;
                }

                // Bez ConfigureAwait(false) – zachowujemy kontekst UI, ale i tak UI-mutacje poniżej są przez Dispatcher
                await _fishing.StopAsync(CancellationToken.None);

                await OnUiAsync(() => Status = _fishing.Status.ToString());
                _logger?.LogInformation("Fishing stopped.");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to stop fishing.");
                await OnUiAsync(() => Events?.Add($"Error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Refreshes the list of candidate processes.
        /// </summary>
        [RelayCommand]
        private async Task RefreshProcessesAsync()
        {
            Processes.Clear();
            var list = await _processes.GetCandidatesAsync(CancellationToken.None);
            foreach (var p in list) Processes.Add(p);
            if (Processes.Count > 0) SelectedProcess = Processes[0];
        }
        
        /// <summary>
        /// Posts an action to the UI Dispatcher (no-op if dispatcher is unavailable).
        /// </summary>
        /// <param name="action">Action to execute on UI thread.</param>
        private static Task OnUiAsync(Action action)
        {
            var disp = Application.Current?.Dispatcher;
            if (disp is null || disp.CheckAccess())
            {
                action();
                return Task.CompletedTask;
            }
            return disp.InvokeAsync(action).Task;
        }

        /// <summary>
        /// Consumes capability events and updates UI (akcje wykonuje FishingActionExecutor).
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        private async Task ConsumeEventsAsync(CancellationToken ct)
        {
            await foreach (var e in _fishing.Events(ct))
            {
                Events.Add($"{e.Type}: {e.Message}");
                Status = _fishing.Status.ToString();
                // UWAGA: nie wywołujemy tutaj HandleCast/HandleSpace — robi to FishingActionExecutor.
            }
        }

        /// <summary>
        /// Called when SelectedBait changes; updates shared session state.
        /// </summary>
        /// <param name="value">New bait option.</param>
        partial void OnSelectedBaitChanged(BaitOption? value)
            => _session.SelectedBait = value;

        /// <summary>
        /// Called when SelectedProcess changes; updates shared session state.
        /// </summary>
        /// <param name="value">New process option.</param>
        partial void OnSelectedProcessChanged(ProcessOption? value)
            => _session.SelectedProcess = value;

        /// <summary>
        /// Maps a WPF key to a numeric bait slot (1..4).
        /// </summary>
        /// <param name="key">WPF key.</param>
        /// <returns>Slot number or null if unsupported.</returns>
        private static int? MapWpfKeyToSlot(Key key) => key switch
        {
            Key.D1 => 1,
            Key.D2 => 2,
            Key.D3 => 3,
            Key.D4 => 4,
            _ => (int?)null
        };
    }
}
