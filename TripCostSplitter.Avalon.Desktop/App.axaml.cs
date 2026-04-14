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
using TripCostSplitter.Core.Services;

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
        serviceCollection.AddSingleton<IDataService, DesktopDataService>();

        Services = serviceCollection.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainViewModel = Services?.GetRequiredService<MainViewModel>();
            var mainView = Services?.GetRequiredService<MainView>();
            if (mainView != null)
                mainView.DataContext = mainViewModel;
                
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel,
                Content = mainView
            };
            
            _ = mainViewModel?.InitializeAsync();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
