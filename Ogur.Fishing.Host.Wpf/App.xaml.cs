using System.Configuration;
using System.Data;
using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Ogur.Fishing.Host.Wpf.Navigation;
using Ogur.Fishing.Host.Wpf.ViewModels;
using Ogur.Fishing.Host.Wpf.Views;
using Ogur.Fishing.Host.Wpf;
using Ogur.Infrastructure.Input;
using Ogur.Infrastructure.Screen;
using Ogur.Infrastructure.Ocr;
using ogur.abstractions;
using ogur.abstractions.Primitives;
using Ogur.Capabilities.Fishing;


namespace Ogur.Fishing.Host.Wpf;

/// <summary>
/// WPF App.
/// </summary>
public partial class App : Application
{
    private IHost? _host;

    /// <inheritdoc />
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _host = AppStartup.BuildHost();
        AppStartup.Run(this, _host);
    }
}