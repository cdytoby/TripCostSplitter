using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using TripCostSplitter.Avalon.Desktop.Views;
using TripCostSplitter.Avalon.Desktop.Services;
using TripCostSplitter.Avalon.ViewModels;
using TripCostSplitter.Avalon.Views;
using TripCostSplitter.Core;

namespace TripCostSplitter.Avalon.Desktop;

public partial class App : Application
{
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTripCostSplitterServices();
        
        // Register platform-specific services
        serviceCollection.AddSingleton<ITravelDataService, DesktopTravelDataService>();

        Services = serviceCollection.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainViewModel = Services?.GetRequiredService<MainViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
            
            _ = mainViewModel?.InitializeAsync();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
