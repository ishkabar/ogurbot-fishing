using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Ogur.Capabilities.Fishing;
using Ogur.Fishing.Host.Wpf.Services;
using Ogur.Abstractions;
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


        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="fishing">Fishing capability.</param>
        /// <param name="sp">Service provider.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="baits">Bait catalog (slots).</param>
        /// <param name="processes">Process query service.</param>
        public MainViewModel(
            FishingCapability fishing,
            IServiceProvider sp,
            ILogger<MainViewModel> logger,
            IBaitCatalog baits,
            IProcessQueryService processes,
            ISessionState session)
        {
            _fishing = fishing;
            _sp = sp;
            _logger = logger;
            _baits = baits;
            _processes = processes;
            _session = session;


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
            try
            {
                await _fishing.StartAsync(
                    new CapabilityStartContext(_sp),
                    CancellationToken.None);

                Status = _fishing.Status.ToString();
                _logger.LogInformation("Fishing started with bait slot {Slot} on PID {Pid}", SelectedBait?.Id, SelectedProcess?.Pid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start fishing.");
                Events.Add($"Error: {ex.Message}");
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
                await _fishing.StopAsync(CancellationToken.None);
                Status = _fishing.Status.ToString();
                _logger.LogInformation("Fishing stopped.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to stop fishing.");
                Events.Add($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Executes a temporary test action.
        /// </summary>
        [RelayCommand]
        private void Test()
        {
            Events.Add($"TEST @ {DateTime.Now:HH:mm:ss}");
            _logger.LogInformation("Test command executed.");
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
        /// Consumes capability events and updates the UI state.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        private async Task ConsumeEventsAsync(CancellationToken ct)
        {
            await foreach (var e in _fishing.Events(ct))
            {
                Events.Add($"{e.Type}: {e.Message}");
                Status = _fishing.Status.ToString();
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
    }
}
