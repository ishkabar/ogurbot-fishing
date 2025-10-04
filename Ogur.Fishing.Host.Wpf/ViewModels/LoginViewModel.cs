using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Ogur.Fishing.Host.Wpf.Navigation;
using Ogur.Fishing.Host.Wpf.Views;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Ogur.Fishing.Host.Wpf.ViewModels.Messages;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Ogur.Fishing.Host.Wpf.Services;
using Ogur.Fishing.Host.Wpf.ViewModels.Messages;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Ogur.Fishing.Host.Wpf.ViewModels.Messages;

using Microsoft.Extensions.Logging;


namespace Ogur.Fishing.Host.Wpf.ViewModels;


/// <summary>
/// ViewModel that handles user login flow for the host application.
/// </summary>
public partial class LoginViewModel : ObservableObject
{
    private readonly ILogger<LoginViewModel> _logger;
    private readonly IAuthService _authService;
    private readonly IMessenger _messenger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="authService">Authentication service.</param>
    /// <param name="messenger">Messenger for publishing application messages.</param>
    public LoginViewModel(ILogger<LoginViewModel> logger, IAuthService authService, IMessenger messenger)
    {
        _logger = logger;
        _authService = authService;
        _messenger = messenger;
    }

    /// <summary>
    /// Gets or sets the username entered by the user.
    /// </summary>
    [ObservableProperty]
    private string? username;

    /// <summary>
    /// Gets or sets the password bound via attached behavior.
    /// </summary>
    [ObservableProperty]
    private string? password;

    partial void OnUsernameChanged(string? value) => LoginCommand.NotifyCanExecuteChanged();
    partial void OnPasswordChanged(string? value) => LoginCommand.NotifyCanExecuteChanged();

    /// <summary>
    /// Command that performs the sign-in operation using the provided credentials.
    /// Enabled only when both username and password are provided.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync(CancellationToken ct)
    {
        _logger.LogInformation("Attempting login for user {User}", Username);

        var ok = await _authService.AuthenticateAsync(Username ?? string.Empty, Password ?? string.Empty, ct).ConfigureAwait(false);
        if (!ok)
        {
            _logger.LogWarning("Login failed for user {User}", Username);
            return;
        }

        _logger.LogInformation("Login succeeded for user {User}", Username);

        // Publish app-flow event → AppFlowCoordinator will navigate further.
        _messenger.Send(new LoginSucceededMessage(Username ?? string.Empty));
    }

    /// <summary>
    /// Determines whether the login command can execute based on input completeness.
    /// </summary>
    /// <returns>True if both username and password are non-empty; otherwise false.</returns>
    private bool CanLogin() =>
        !string.IsNullOrWhiteSpace(Username) &&
        !string.IsNullOrEmpty(Password);
}