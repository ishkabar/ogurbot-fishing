using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Ogur.Fishing.Host.Wpf.Navigation;
using Ogur.Fishing.Host.Wpf.Views;
using System.Windows;
using System.Windows.Controls;


namespace Ogur.Fishing.Host.Wpf.Views;

/// <summary>
/// Main shell window that hosts application views.
/// </summary>
public partial class ShellWindow : BaseWindow
{
    /// <summary>
    /// Gets the content presenter used by navigation service to inject views.
    /// </summary>
    public ContentPresenter ContentPresenter => AppContent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellWindow"/> class.
    /// </summary>
    public ShellWindow()
    {
        InitializeComponent();
    }
}