// File: Ogur.Fishing.Host.Wpf/ViewModels/MainViewModel.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.ViewModels

using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions;
using Ogur.Abstractions.Events;
using Ogur.Capabilities.Fishing;
using Ogur.Fishing.Host.Wpf.Services;
using Ogur.Fishing.Host.Wpf.Services.Models;

namespace Ogur.Fishing.Host.Wpf.ViewModels;

/// <summary>
/// Main view model for the fishing automation UI.
/// </summary>
public sealed partial class MainViewModel : ObservableObject
{
    private readonly FishingCapability _fishing;
    private readonly IServiceProvider _sp;
    private readonly ILogger<MainViewModel> _logger;
    private readonly IBaitCatalog _baits;
    private readonly IProcessQueryService _processes;
    private readonly ISessionState _session;
    private readonly IEventBus _eventBus;
    private readonly CancellationTokenSource _cts = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    /// <param name="fishing">Fishing capability instance.</param>
    /// <param name="sp">Service provider for dependency resolution.</param>
    /// <param name="logger">Logger instance.</param>
    /// <param name="baits">Bait catalog service.</param>
    /// <param name="processes">Process query service.</param>
    /// <param name="session">Session state singleton.</param>
    /// <param name="eventBus">Event bus for UI event updates.</param>
    public MainViewModel(
        FishingCapability fishing,
        IServiceProvider sp,
        ILogger<MainViewModel> logger,
        IBaitCatalog baits,
        IProcessQueryService processes,
        ISessionState session,
        IEventBus eventBus)
    {
        _fishing = fishing;
        _sp = sp;
        _logger = logger;
        _baits = baits;
        _processes = processes;
        _session = session;
        _eventBus = eventBus;

        BaitItems = new ObservableCollection<BaitOption>(_baits.GetAll());
        Events = new ObservableCollection<string>();
        
        _session.MemoryAddress = 0x0CCF203E;
        _logger.LogInformation("Default memory address set: 0x{Addr:X}", _session.MemoryAddress);

        ParseAndSetMemoryAddress(_memoryAddress);

        
        _ = ConsumeEventsAsync(_cts.Token);
        _ = RefreshProcessesAsync();
    }

    [ObservableProperty]
    private bool _isFishing;

    [ObservableProperty]
    private string _status = "Stopped";

    [ObservableProperty]
    private BaitOption? _selectedBait;

    [ObservableProperty]
    private ProcessOption? _selectedProcess;

    [ObservableProperty]
    private string _memoryAddress = "0x0CCF203E";

    private void ParseAndSetMemoryAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _session.MemoryAddress = 0;
            _logger.LogWarning("Memory address cleared");
            return;
        }
    
        // Remove "0x" prefix
        var hexString = value.Replace("0x", "").Replace("0X", "").Trim();
    
        if (long.TryParse(hexString, System.Globalization.NumberStyles.HexNumber, null, out long addr))
        {
            _session.MemoryAddress = addr;
            _logger.LogInformation("Memory address set: 0x{Addr:X} ({Addr} decimal)", addr, addr);
        }
        else
        {
            _logger.LogWarning("Invalid hex address format: '{Value}'", value);
            _session.MemoryAddress = 0;
        }
    }
    
    partial void OnMemoryAddressChanged(string value)
    {
        var hexString = value.Replace("0x", "").Replace("0X", "").Trim();
    
        if (long.TryParse(hexString, System.Globalization.NumberStyles.HexNumber, null, out long addr))
        {
            _session.MemoryAddress = addr;
            _logger.LogInformation("Memory address updated: 0x{Addr:X}", addr);
        }
    }

    /// <summary>
    /// Gets the collection of fishing events displayed in the UI.
    /// </summary>
    public ObservableCollection<string> Events { get; }

    /// <summary>
    /// Gets the collection of available bait options.
    /// </summary>
    public ObservableCollection<BaitOption> BaitItems { get; }

    /// <summary>
    /// Gets the collection of available game processes.
    /// </summary>
    public ObservableCollection<ProcessOption> Processes { get; } = new();

    partial void OnIsFishingChanged(bool value)
    {
        StartCommand.NotifyCanExecuteChanged();
        StopCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedProcessChanged(ProcessOption? value)
    {
        _session.SelectedProcess = value;
        StartCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedBaitChanged(BaitOption? value)
    {
        _session.SelectedBait = value;
    }

    /// <summary>
    /// Starts the fishing automation.
    /// </summary>
    /// <returns>Task representing the async operation.</returns>
    [RelayCommand(CanExecute = nameof(CanStart))]
    private async Task StartAsync()
    {
        if (IsFishing)
        {
            return;
        }

        try
        {
            IsFishing = true;

            await _fishing.StartAsync(
                new CapabilityStartContext(_sp),
                CancellationToken.None);

            Status = _fishing.Status.ToString();
            _logger.LogInformation(
                "Fishing started with bait {Bait} on process PID={Pid}, HWND=0x{Hwnd:X}",
                SelectedBait?.DisplayName ?? "none",
                SelectedProcess?.Pid,
                SelectedProcess?.Hwnd);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start fishing");
            Events.Add($"Error: {ex.Message}");
            IsFishing = false;
            Status = "Error";
        }
    }

    /// <summary>
    /// Stops the fishing automation.
    /// </summary>
    /// <returns>Task representing the async operation.</returns>
    [RelayCommand(CanExecute = nameof(CanStop))]
    private async Task StopAsync()
    {
        if (!IsFishing)
        {
            return;
        }

        try
        {
            await _fishing.StopAsync(CancellationToken.None);
            IsFishing = false;
            Status = _fishing.Status.ToString();
            _logger.LogInformation("Fishing stopped");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop fishing");
            Events.Add($"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Determines whether the Start command can execute.
    /// </summary>
    /// <returns>True if fishing can be started; otherwise false.</returns>
    private bool CanStart() => !IsFishing && SelectedProcess is not null;

    /// <summary>
    /// Determines whether the Stop command can execute.
    /// </summary>
    /// <returns>True if fishing can be stopped; otherwise false.</returns>
    private bool CanStop() => IsFishing;

    /// <summary>
    /// Test command for debugging UI interactions.
    /// </summary>
    [RelayCommand]
    private void Test()
    {
        Events.Add($"🧪 TEST @ {DateTime.Now:HH:mm:ss}");
        _logger.LogInformation("Test command executed");
    }

    /// <summary>
    /// Refreshes the list of available game processes.
    /// </summary>
    /// <returns>Task representing the async operation.</returns>
    [RelayCommand]
    private async Task RefreshProcessesAsync()
    {
        try
        {
            Processes.Clear();
            var candidates = await _processes.GetCandidatesAsync(CancellationToken.None);
            
            foreach (var process in candidates)
            {
                Processes.Add(process);
            }

            if (Processes.Count > 0)
            {
                SelectedProcess = Processes[0];
                _logger.LogInformation("Found {Count} game process(es)", Processes.Count);
            }
            else
            {
                _logger.LogWarning("No game processes found");
                Events.Add("No game processes detected");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh processes");
            Events.Add($"Failed to scan processes: {ex.Message}");
        }
    }

    /// <summary>
    /// Consumes events from EventBus and displays them in the UI.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task representing the async operation.</returns>
    private async Task ConsumeEventsAsync(CancellationToken ct)
    {
        try
        {
            await foreach (var e in _eventBus.Subscribe("fishing.*", ct))
            {
                var timestamp = e.Timestamp.ToLocalTime().ToString("HH:mm:ss");
                Events.Add($"[{timestamp}] {e.Type}: {e.Message}");
                Status = _fishing.Status.ToString();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Event consumption cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Event consumption failed");
        }
    }
}