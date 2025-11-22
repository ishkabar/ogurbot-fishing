// File: Ogur.Fishing.Host.Wpf/ViewModels/LoginViewModel.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.ViewModels

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions.Hub;
using Ogur.Fishing.Host.Wpf.ViewModels.Messages;

namespace Ogur.Fishing.Host.Wpf.ViewModels;

/// <summary>
/// ViewModel that handles user login flow for the host application.
/// </summary>
public sealed partial class LoginViewModel : ObservableObject
{
    private readonly ILogger<LoginViewModel> _logger;
    private readonly IAuthService _authService;
    private readonly ILicenseValidator _licenseValidator; 
    private readonly IMessenger _messenger;
    private bool _isClearing;


    public LoginViewModel(
        ILogger<LoginViewModel> logger,
        IAuthService authService,
        ILicenseValidator licenseValidator,
        IMessenger messenger)
    {
        _logger = logger;
        _authService = authService;
        _licenseValidator = licenseValidator;
        _messenger = messenger;
        
        // TODO: HARDCODED DEBUG CREDENTIALS
        Username = "test";
        Password = "231231";

        // TODO: AUTO LOGIN ON STARTUP
        _ = Task.Run(async () =>
        {
            await Task.Delay(500);
            await LoginAsync(CancellationToken.None);
        });
    }

    [ObservableProperty] private string? _username;

    [ObservableProperty] private string? _password;

    private string? _errorMessage;

    /// <summary>
    /// Gets or sets the error message to display.
    /// </summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage != value)
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }
    }

    [ObservableProperty] private bool _isLoading;

    partial void OnUsernameChanged(string? value)
    {
        if (!_isClearing)
            ErrorMessage = null;
    
        LoginCommand.NotifyCanExecuteChanged();
    }

    partial void OnPasswordChanged(string? value)
    {
        if (!_isClearing)
            ErrorMessage = null;
    
        LoginCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            SetError("Please enter username and password");
            return;
        }

        SetError(null);
        SetLoading(true);

        try
        {
            _logger.LogInformation("Attempting login for user: {Username}", Username);

            var result = await _authService.LoginAsync(Username, Password, ct);

            if (result.Success)
            {
                _logger.LogInformation("Login successful, validating license...");
        
                var licenseResult = await _licenseValidator.ValidateAsync(ct);
        
                if (!licenseResult.IsValid)
                {
                    SetError($"License validation failed: {licenseResult.ErrorMessage}");
                    _authService.Logout();
                    return;
                }
        
                _logger.LogInformation("License valid until: {ExpiresAt}", licenseResult.ExpiresAt);
        
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _messenger.Send(new LoginSucceededMessage(result.Username));
                });
            }
            else
            {
                var errorMsg = result.ErrorMessage ?? "Login failed";

                if (errorMsg.StartsWith("{"))
                {
                    try
                    {
                        var json = System.Text.Json.JsonDocument.Parse(errorMsg);
                        if (json.RootElement.TryGetProperty("error", out var errorProp))
                        {
                            errorMsg = errorProp.GetString() ?? errorMsg;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to parse error JSON");
                    }
                }

                SetError(errorMsg);
                _logger.LogWarning("Login failed for user {Username}: {Error}", Username, errorMsg);
            }
        }
        catch (Exception ex)
        {
            SetError($"Login error: {ex.Message}");
            _logger.LogError(ex, "Login exception for user: {Username}", Username);
        }
        finally
        {
            SetLoading(false);
            Application.Current.Dispatcher.Invoke(() =>
            {
                _isClearing = true;
                Password = string.Empty;
                _isClearing = false;
            });
        }
    }

    private bool CanLogin() =>
        !string.IsNullOrWhiteSpace(Username) &&
        !string.IsNullOrWhiteSpace(Password) &&
        !IsLoading;

    private void SetError(string? message)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            ErrorMessage = message;
        });
    }

    private void SetLoading(bool loading)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            IsLoading = loading;
            LoginCommand.NotifyCanExecuteChanged();
        });
    }
}