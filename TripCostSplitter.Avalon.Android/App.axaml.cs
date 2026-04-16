using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using TripCostSplitter.Avalon.Android.Services;
using TripCostSplitter.Avalon.Views;
using TripCostSplitter.Core;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.Avalon.Android;

public partial class App : Avalonia.Application
{
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        ServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddTripCostSplitterServices();
        
        // Register platform-specific services
        serviceCollection.AddSingleton<IDataService, AndroidDataService>();

        Services = serviceCollection.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IActivityApplicationLifetime singleView)
        {
            MainView? mainView = Services?.GetRequiredService<MainView>();
            if (mainView != null)
            {
                singleView.MainViewFactory = () => mainView;
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}
