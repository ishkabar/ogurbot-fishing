using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Ogur.Fishing.Host.Wpf.Services.Models;

namespace Ogur.Fishing.Host.Wpf.Services.Implementations;


/// <summary>
/// Hotkey listener that waits for the next KeyDown reported by the WPF input manager.
/// </summary>
public sealed class HotkeyListener : IHotkeyListener
{
    /// <inheritdoc />
    public Task<KeyGesture> CaptureNextAsync(CancellationToken ct)
    {
        var tcs = new TaskCompletionSource<KeyGesture>(TaskCreationOptions.RunContinuationsAsynchronously);

        // This must run on UI thread where InputManager is alive.
        void Handler(object? s, PreProcessInputEventArgs e)
        {
            if (e?.StagingItem?.Input is not KeyEventArgs kea) return;
            if (kea.RoutedEvent != Keyboard.KeyDownEvent) return;

            var key = kea.Key == Key.System ? kea.SystemKey : kea.Key;
            var mods = Keyboard.Modifiers;
            InputManager.Current.PreProcessInput -= Handler;
            if (!tcs.Task.IsCompleted)
            {
                tcs.SetResult(new KeyGesture(key, mods));
            }
        }

        InputManager.Current.PreProcessInput += Handler;

        if (ct.CanBeCanceled)
        {
            ct.Register(state =>
            {
                InputManager.Current.PreProcessInput -= Handler;
                var t = (TaskCompletionSource<KeyGesture>)state!;
                if (!t.Task.IsCompleted)
                {
                    t.SetCanceled();
                }
            }, tcs);
        }

        // Safety timeout to avoid eternal wait if needed.
        _ = Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
        {
            await Task.Delay(TimeSpan.FromMinutes(5), CancellationToken.None);
            if (!tcs.Task.IsCompleted)
            {
                InputManager.Current.PreProcessInput -= Handler;
                tcs.TrySetException(new TimeoutException("Hotkey capture timed out."));
            }
        });

        return tcs.Task;
    }
}