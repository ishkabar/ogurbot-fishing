// File: Ogur.Fishing.Host.Wpf/Services/HubIntegration/HubCommandHandler.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.Services.HubIntegration

using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions.Hub;

namespace Ogur.Fishing.Host.Wpf.Services.HubIntegration;

/// <summary>
/// Handles commands received from Ogur.Hub via SignalR.
/// </summary>
public sealed class HubCommandHandler
{
    private readonly IHubClient _hubClient;
    private readonly ILicenseManager _licenseManager;
    private readonly ILogger<HubCommandHandler> _logger;

    public HubCommandHandler(
        IHubClient hubClient,
        ILicenseManager licenseManager,
        ILogger<HubCommandHandler> logger)
    {
        _hubClient = hubClient;
        _licenseManager = licenseManager;
        _logger = logger;
    }

    public void StartListening()
    {
        _ = Task.Run(async () =>
        {
            await foreach (var command in _hubClient.ListenForCommandsAsync(CancellationToken.None))
            {
                await HandleCommandAsync(command);
            }
        });
        _logger.LogInformation("Hub command handler started");
    }

    public void StopListening()
    {
        _logger.LogInformation("Hub command handler stopped");
    }

    private async Task HandleCommandAsync(HubCommand command)
    {
        _logger.LogInformation("Received hub command: {CommandType} (ID: {CommandId})",
            command.Type, command.CommandId);

        try
        {
            switch (command.Type)
            {
                case HubCommandType.Logout:
                    await HandleLogoutCommandAsync(command);
                    break;

                case HubCommandType.BlockDevice:
                    await HandleBlockDeviceCommandAsync(command);
                    break;

                case HubCommandType.Notify:
                    await HandleNotifyCommandAsync(command);
                    break;

                case HubCommandType.ForceUpdate:
                    await HandleForceUpdateCommandAsync(command);
                    break;

                case HubCommandType.RefreshLicense:
                    await HandleRefreshLicenseCommandAsync(command);
                    break;

                case HubCommandType.Custom:
                    await HandleCustomCommandAsync(command);
                    break;

                default:
                    _logger.LogWarning("Unknown command type: {CommandType}", command.Type);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle command {CommandId}", command.CommandId);
        }
    }

    private async Task HandleLogoutCommandAsync(HubCommand command)
    {
        _logger.LogWarning("Logout command received - shutting down application");

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            MessageBox.Show(
                "Your session has been terminated by the administrator.",
                "Session Ended",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            Application.Current.Shutdown();
        });
    }

    private async Task HandleBlockDeviceCommandAsync(HubCommand command)
    {
        _logger.LogWarning("Block device command received - shutting down application");

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            MessageBox.Show(
                "This device has been blocked. Please contact support.",
                "Device Blocked",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Application.Current.Shutdown();
        });
    }

    private async Task HandleNotifyCommandAsync(HubCommand command)
    {
        var message = "Notification from hub";
        
        if (command.Payload is JsonElement element && element.ValueKind == JsonValueKind.Object)
        {
            if (element.TryGetProperty("message", out var msgProp))
            {
                message = msgProp.GetString() ?? message;
            }
        }
        
        _logger.LogInformation("Notification: {Message}", message);

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            MessageBox.Show(
                message,
                "Notification",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        });
    }

    private async Task HandleForceUpdateCommandAsync(HubCommand command)
    {
        _logger.LogInformation("Force update command received");

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var result = MessageBox.Show(
                "A critical update is available and must be installed. The application will restart.",
                "Update Required",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        });
    }

    private async Task HandleRefreshLicenseCommandAsync(HubCommand command)
    {
        _logger.LogInformation("Refresh license command received");
        await _licenseManager.ValidateLicenseAsync();
    }

    private Task HandleCustomCommandAsync(HubCommand command)
    {
        var action = "unknown";
        
        if (command.Payload is JsonElement element && element.ValueKind == JsonValueKind.Object)
        {
            if (element.TryGetProperty("action", out var actionProp))
            {
                action = actionProp.GetString() ?? action;
            }
        }
        
        _logger.LogInformation("Custom command received: {Action}", action);
        return Task.CompletedTask;
    }
}