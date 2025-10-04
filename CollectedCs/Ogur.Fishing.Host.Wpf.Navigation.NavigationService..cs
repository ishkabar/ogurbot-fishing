using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ogur.Fishing.Host.Wpf.Views;

namespace Ogur.Fishing.Host.Wpf.Navigation
{
    /// <summary>
    /// Default navigation service that swaps content inside ShellWindow.
    /// Minimal and defensive: looks for PART_ContentHost, falls back to ContentControl or to ShellWindow.Content.
    /// </summary>
    public sealed class NavigationService : INavigationService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<NavigationService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationService"/> class.
        /// </summary>
        /// <param name="sp">Service provider used to resolve views.</param>
        /// <param name="logger">Logger instance.</param>
        public NavigationService(IServiceProvider sp, ILogger<NavigationService> logger)
        {
            _sp = sp ?? throw new ArgumentNullException(nameof(sp));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Displays a view of the specified type inside the shell region.
        /// Attempts to resolve a named ContentPresenter ("PART_ContentHost") first, then falls back to ContentControl,
        /// and as a last resort sets ShellWindow.Content directly.
        /// </summary>
        /// <typeparam name="TView">The WPF view type to display. Must be registered in DI.</typeparam>
        public void Show<TView>() where TView : FrameworkElement
        {
            var dispatcher = Application.Current?.Dispatcher
                             ?? throw new InvalidOperationException("WPF Application dispatcher not available.");

            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke(() => Show<TView>());
                return;
            }

            if (Application.Current?.MainWindow is not ShellWindow shell)
                throw new InvalidOperationException("ShellWindow not found as Application.Current.MainWindow.");

            // Try find named PART_ContentHost inside ShellWindow (recommended control name)
            object? namedHost = null;
            try
            {
                namedHost = shell.FindName("PART_ContentHost");
            }
            catch
            {
                // Ignore; FindName can throw if template not applied — we'll fallback below.
            }

            // Resolve view instance from DI
            var view = _sp.GetService(typeof(TView)) as FrameworkElement ?? Activator.CreateInstance(typeof(TView)) as FrameworkElement;
            if (view is null)
            {
                _logger.LogError("Unable to resolve view of type {ViewType}. Ensure it is registered in DI or has a parameterless constructor.", typeof(TView).FullName);
                throw new InvalidOperationException($"Unable to create view of type {typeof(TView).FullName}.");
            }

            _logger.LogDebug("Navigating to {View}. NamedHostFound={HasNamedHost}, ShellContentType={ShellContentType}.",
                typeof(TView).Name,
                namedHost != null,
                shell.Content?.GetType().Name ?? "<null>");

            // If the named host is a ContentPresenter, set its Content.
            if (namedHost is ContentPresenter cp)
            {
                cp.Content = view;
                return;
            }

            // If shell.Content is a ContentControl (e.g. a Panel or ContentControl wrapper), set its Content.
            if (shell.Content is ContentControl cc)
            {
                cc.Content = view;
                return;
            }

            // Last resort: set the window content directly.
            shell.Content = view;
        }
    }
}
