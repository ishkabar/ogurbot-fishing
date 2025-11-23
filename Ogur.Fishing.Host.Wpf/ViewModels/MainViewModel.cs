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
using Ogur.Abstractions.Metin;
using Ogur.Abstractions.Events;
using Ogur.Capabilities.Fishing;
using Ogur.Infrastructure.Signals;
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


        _logger.LogInformation("Default memory address set: 0x{Addr:X}", _session.MemoryAddress);


        _ = ConsumeEventsAsync(_cts.Token);
        _ = RefreshProcessesAsync();
    }

    [ObservableProperty] private bool _isFishing;

    [ObservableProperty] private string _status = "Stopped";

    [ObservableProperty] private BaitOption? _selectedBait;

    [ObservableProperty] private ProcessOption? _selectedProcess;

    [ObservableProperty] private string _detectedAddress = "Not detected";

    [ObservableProperty] private bool _isDetecting;

    [ObservableProperty] private int _detectionProgress;

    [ObservableProperty] private string _detectionStatus = "";
    
    [ObservableProperty] private bool _hasDetectedAddress;
    
    [ObservableProperty] private bool _isRefreshing;

    partial void OnDetectedAddressChanged(string value)
    {
        HasDetectedAddress = !string.IsNullOrEmpty(value) && value != "Not detected";
        DetectCommand.NotifyCanExecuteChanged();
        StartCommand.NotifyCanExecuteChanged();
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
        DetectCommand.NotifyCanExecuteChanged();
        StartCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedBaitChanged(BaitOption? value)
    {
        _session.SelectedBait = value;
        StartCommand.NotifyCanExecuteChanged();
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
    private bool CanStart() => 
        !IsFishing && 
        SelectedProcess is not null && 
        SelectedBait is not null &&
        HasDetectedAddress;

    /// <summary>
    /// Determines whether the Stop command can execute.
    /// </summary>
    /// <returns>True if fishing can be stopped; otherwise false.</returns>
    private bool CanStop() => IsFishing;

    /// <summary>
    /// Test command for debugging UI interactions.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanDetect))]
    private async Task DetectAsync()
    {
        try
        {
            IsDetecting = true;
            DetectionProgress = 0;
            DetectionStatus = "Opening process...";
        
            _eventBus.Publish("detection.start", "Starting chat buffer detection");

            var signalSource = _sp.GetRequiredService<IFishingSignalSource>() as MemoryBiteSignalSource;
        
            if (signalSource is null)
            {
                _eventBus.Publish("detection.error", "MemoryBiteSignalSource not available");
                return;
            }

            // Symuluj postęp
            var progressTask = Task.Run(async () =>
            {
                // Snapshots: 0-60%
                for (int i = 0; i <= 60; i++)
                {
                    DetectionProgress = i;
                    DetectionStatus = $"Taking snapshots ({i}/60)...";
                    await Task.Delay(50);
                }

                // Analyzing: 60-85%
                for (int i = 60; i <= 85; i++)
                {
                    DetectionProgress = i;
                    DetectionStatus = "Analyzing changes...";
                    await Task.Delay(25);
                }

                // Validating: 85-95%
                for (int i = 85; i <= 95; i++)
                {
                    DetectionProgress = i;
                    DetectionStatus = "Validating regions...";
                    await Task.Delay(100);
                }
            });

            // Wywołaj detection - TO trwa ~6-7 sekund
            _ = await signalSource.WaitForBiteAsync(TimeSpan.FromSeconds(1), _cts.Token);

            // Poczekaj na progress task
            await progressTask;

            // Teraz dopiero 100%
            DetectionProgress = 100;
            DetectionStatus = "Complete!";

            _eventBus.Publish("detection.complete", "Chat buffer detected");

            if (_session.MemoryAddress != 0)
            {
                DetectedAddress = $"0x{_session.MemoryAddress:X8}";
                _eventBus.Publish("detection.address", $"Address: 0x{_session.MemoryAddress:X8}");
            }

            await Task.Delay(800);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Detection failed");
            _eventBus.Publish("detection.error", ex.Message);
            DetectionStatus = "Failed!";
        }
        finally
        {
            IsDetecting = false;
        }
    }
    private bool CanDetect() => 
        !HasDetectedAddress && 
        SelectedProcess is not null && 
        !IsFishing;
    /// <summary>
    /// Refreshes the list of available game processes.
    /// </summary>
    /// <returns>Task representing the async operation.</returns>
    [RelayCommand]
    private async Task RefreshProcessesAsync()
    {
        try
        {
        IsRefreshing = true;
        
            Processes.Clear();
            SelectedProcess = null;
            
            var candidates = await _processes.GetCandidatesAsync(CancellationToken.None);

            foreach (var process in candidates)
            {
                Processes.Add(process);
            }

            if (Processes.Count > 0)
            {
                //SelectedProcess = Processes[0];
                _logger.LogInformation("Found {Count} game process(es)", Processes.Count);
            }
            else
            {
                _logger.LogWarning("No game processes found");
                _eventBus.Publish("process.scan", "No game processes detected");
            }
            
            await Task.Delay(300);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh processes");
            _eventBus.Publish("process.error", $"Failed to scan: {ex.Message}");
        }
        finally
    {
        IsRefreshing = false;
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

                /*if (_session.MemoryAddress != 0)
                {
                    DetectedAddress = $"0x{_session.MemoryAddress:X8}";
                }*/
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