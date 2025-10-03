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
using ogur.abstractions;
// Aliases to avoid type conflicts across projects
using BaitOption = Ogur.Fishing.Host.Wpf.Services.Models.BaitOption;
using ProcessOption = Ogur.Fishing.Host.Wpf.Services.Models.ProcessOption;


namespace Ogur.Fishing.Host.Wpf.ViewModels;


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
    private readonly IHotkeyListener _hotkey;
    private readonly CancellationTokenSource _cts = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    /// <param name="fishing">Fishing capability.</param>
    /// <param name="sp">Service provider.</param>
    /// <param name="logger">Logger.</param>
    /// <param name="baits">Bait catalog (slots).</param>
    /// <param name="processes">Process query service.</param>
    /// <param name="hotkey">Hotkey listener.</param>
    public MainViewModel(
        FishingCapability fishing,
        IServiceProvider sp,
        ILogger<MainViewModel> logger,
        IBaitCatalog baits,
        IProcessQueryService processes,
        IHotkeyListener hotkey)
    {
        _fishing = fishing;
        _sp = sp;
        _logger = logger;
        _baits = baits;
        _processes = processes;
        _hotkey = hotkey;

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
    /// Gets or sets the configured capture/listen hotkey text.
    /// </summary>
    [ObservableProperty] private string _hotkeyText = "None";

    /// <summary>
    /// Gets a value indicating whether we are listening for a hotkey now.
    /// </summary>
    [ObservableProperty] private bool _isListeningHotkey;

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
                new CapabilityStartContext
                {
                    Services = _sp,
                    StoppingToken = _cts.Token
                },
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
    /// Executes a temporary test action (to be removed).
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
    /// Captures the next pressed key as a hotkey.
    /// </summary>
    [RelayCommand]
    private async Task ListenHotkeyAsync()
    {
        if (IsListeningHotkey) return;
        IsListeningHotkey = true;
        try
        {
            var gesture = await _hotkey.CaptureNextAsync(_cts.Token);
            var mods = gesture.Modifiers is ModifierKeys.None ? string.Empty : $"{gesture.Modifiers}+";
            HotkeyText = $"{mods}{gesture.Key}";
            _logger.LogInformation("Captured hotkey: {Hotkey}", HotkeyText);
        }
        finally
        {
            IsListeningHotkey = false;
        }
    }

    private async Task ConsumeEventsAsync(CancellationToken ct)
    {
        await foreach (var e in _fishing.Events(ct))
        {
            Events.Add($"{e.Type}: {e.Message}");
            Status = _fishing.Status.ToString();
        }
    }
}