using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using TripCostSplitter.Avalon.Desktop.Views;
using TripCostSplitter.Avalon.Desktop.Services;
using TripCostSplitter.Avalon.Views;
using TripCostSplitter.Core;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.Avalon.Desktop;

public partial class App : Application
{
    private IServiceProvider? serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        ServiceCollection serviceCollection = new ();
        serviceCollection.AddTripCostSplitterServices();
        
        // Register platform-specific services
        serviceCollection.AddSingleton<IDataService, DesktopDataService>();

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainView mainView = serviceProvider!.GetRequiredService<MainView>();
                
            desktop.MainWindow = new MainWindow
            {
                Content = mainView
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
