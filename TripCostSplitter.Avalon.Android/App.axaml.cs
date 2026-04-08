using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using TripCostSplitter.Avalon.Android.Services;
using TripCostSplitter.Avalon.ViewModels;
using TripCostSplitter.Avalon.Views;
using TripCostSplitter.Core;

namespace TripCostSplitter.Avalon.Android;

public partial class App : Avalonia.Application
{
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTripCostSplitterServices();
        
        // Register platform-specific services
        serviceCollection.AddSingleton<ITravelDataService, AndroidTravelDataService>();

        Services = serviceCollection.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            var mainViewModel = Services?.GetRequiredService<MainViewModel>();
            singleView.MainView = new MainView
            {
                DataContext = mainViewModel
            };
            
            _ = mainViewModel?.InitializeAsync();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
