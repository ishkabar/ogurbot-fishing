// File: Ogur.Fishing.Host.Wpf/Services/HubIntegration/LicenseManager.cs
// Project: Ogur.Fishing.Host.Wpf
// Namespace: Ogur.Fishing.Host.Wpf.Services.HubIntegration

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ogur.Abstractions.Hub;
using Ogur.Fishing.Host.Wpf.Configuration;

namespace Ogur.Fishing.Host.Wpf.Services.HubIntegration;

/// <summary>
/// Event args for license status changes.
/// </summary>
public sealed class LicenseStatusChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets a value indicating whether license is valid.
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Gets the status message.
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Gets the expiration date.
    /// </summary>
    public DateTime? ExpiresAt { get; init; }
}

/// <summary>
/// Manages license validation and monitoring.
/// </summary>
public interface ILicenseManager
{
    /// <summary>
    /// Gets a value indicating whether license is valid.
    /// </summary>
    bool IsLicenseValid { get; }

    /// <summary>
    /// Gets the current license status.
    /// </summary>
    string LicenseStatus { get; }

    /// <summary>
    /// Gets the license expiration date.
    /// </summary>
    DateTime? LicenseExpiresAt { get; }

    /// <summary>
    /// Validates the license asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if license is valid.</returns>
    Task<bool> ValidateLicenseAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts periodic license validation.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task StartPeriodicValidationAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Event raised when license status changes.
    /// </summary>
    event EventHandler<LicenseStatusChangedEventArgs>? LicenseStatusChanged;
}

/// <summary>
/// Manages license validation and monitoring.
/// </summary>
public sealed class LicenseManager : ILicenseManager
{
    private readonly ILicenseValidator _licenseValidator;
    private readonly ILogger<LicenseManager> _logger;
    private readonly LicenseOptions _options;
    private bool _isValid;
    private string _status = "Not validated";
    private DateTime? _expiresAt;

    /// <summary>
    /// Initializes a new instance of the <see cref="LicenseManager"/> class.
    /// </summary>
    /// <param name="licenseValidator">License validator.</param>
    /// <param name="options">License options.</param>
    /// <param name="logger">Logger.</param>
    public LicenseManager(
        ILicenseValidator licenseValidator,
        IOptions<LicenseOptions> options,
        ILogger<LicenseManager> logger)
    {
        _licenseValidator = licenseValidator;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public bool IsLicenseValid => _isValid;

    /// <inheritdoc />
    public string LicenseStatus => _status;

    /// <inheritdoc />
    public DateTime? LicenseExpiresAt => _expiresAt;

    /// <inheritdoc />
    public event EventHandler<LicenseStatusChangedEventArgs>? LicenseStatusChanged;

    /// <inheritdoc />
    public async Task<bool> ValidateLicenseAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating license with Ogur.Hub");

            var result = await _licenseValidator.ValidateAsync(cancellationToken);

            var wasValid = _isValid;
            _isValid = result.IsValid;
            
            // Build status message from result
            if (result.IsValid)
            {
                _status = $"Valid (expires: {result.ExpiresAt:yyyy-MM-dd}, devices: {result.RegisteredDevices}/{result.MaxDevices})";
            }
            else
            {
                _status = result.ErrorMessage ?? result.Error?.ToString() ?? "Invalid";
            }
            
            _expiresAt = result.ExpiresAt;

            if (wasValid != _isValid)
            {
                _logger.LogInformation("License status changed: {Status}", _status);
                OnLicenseStatusChanged(new LicenseStatusChangedEventArgs
                {
                    IsValid = _isValid,
                    Status = _status,
                    ExpiresAt = _expiresAt
                });
            }

            if (_isValid)
            {
                _logger.LogInformation("License valid until: {ExpiresAt}, devices: {Registered}/{Max}", 
                    _expiresAt, result.RegisteredDevices, result.MaxDevices);
            }
            else
            {
                _logger.LogWarning("License validation failed: {Status} (Error: {Error})", 
                    _status, result.Error);
            }

            return _isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "License validation failed with exception");
            _status = $"Validation error: {ex.Message}";
            _isValid = false;
            return false;
        }
    }

    /// <inheritdoc />
    public async Task StartPeriodicValidationAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting periodic license validation (interval: {Interval} minutes)",
            _options.CheckIntervalMinutes);

        await ValidateLicenseAsync(cancellationToken);

        _ = Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(_options.CheckIntervalMinutes), cancellationToken);
                    await ValidateLicenseAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during periodic license validation");
                }
            }
        }, cancellationToken);
    }

    private void OnLicenseStatusChanged(LicenseStatusChangedEventArgs e)
    {
        LicenseStatusChanged?.Invoke(this, e);
    }
}