using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Bot.Capabilities.Fishing;
using Bot.Abstractions;


namespace Ogur.Fishing.Host.Wpf.ViewModels;


/// <summary>
/// Main dashboard VM controlling Fishing capability.
/// </summary>
public sealed partial class MainViewModel : ObservableObject
{
    private readonly FishingCapability _fishing;
    private readonly CancellationTokenSource _cts = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    /// <param name="fishing">Fishing capability.</param>
    public MainViewModel(FishingCapability fishing)
    {
        _fishing = fishing;
        _ = ConsumeEventsAsync(_cts.Token);
    }

    /// <summary>
    /// Gets the current FSM-like status text.
    /// </summary>
    [ObservableProperty]
    private string _status = "Stopped";

    /// <summary>
    /// Gets the live log of capability events.
    /// </summary>
    public ObservableCollection<string> Events { get; } = new();

    /// <summary>
    /// Starts fishing capability.
    /// </summary>
    public IAsyncRelayCommand StartCommand => new AsyncRelayCommand(StartAsync);

    /// <summary>
    /// Stops fishing capability.
    /// </summary>
    public IAsyncRelayCommand StopCommand => new AsyncRelayCommand(StopAsync);

    private async Task StartAsync()
    {
        await _fishing.StartAsync(CapabilityStartContext.Empty, CancellationToken.None);
        Status = _fishing.Status.ToString();
    }

    private async Task StopAsync()
    {
        await _fishing.StopAsync(CancellationToken.None);
        Status = _fishing.Status.ToString();
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