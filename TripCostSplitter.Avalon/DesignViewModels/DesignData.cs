using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using TripCostSplitter.AppBase.ViewModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.Avalon.DesignViewModels;

public static class DesignData
{
    private static IServiceProvider serviceProvider { get; } = GetProvider();
    
    public static SettingsViewModel ExampleSettingsViewModel { get; } = GetSettingsViewModel();
    
    private static IServiceProvider GetProvider()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddAvalonServices();
        serviceCollection.AddSingleton<IDataService, MockDataService>();
        return serviceCollection.BuildServiceProvider();
    }
    
    private static SettingsViewModel GetSettingsViewModel()
    {
        SettingsViewModel settingsViewModel = serviceProvider.GetService<SettingsViewModel>()!;
        return settingsViewModel;
    }
}