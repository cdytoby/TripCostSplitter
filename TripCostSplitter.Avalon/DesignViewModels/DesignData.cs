using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.AppBase.ViewModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.Avalon.DesignViewModels;

public static class DesignData
{
    private static IServiceProvider serviceProvider { get; } = GetProvider();
    
    public static SettingsViewModel ExampleSettingsViewModel { get; } = GetSettingsViewModel();
    
    public static TravelListViewModel TravelListViewModelDesign { get; } = GetTravelListViewModel();
    
    public static TravelDetailViewModel TravelDetailViewModelDesign { get; } = GetTravelDetailViewModel();
    
    public static TransactionListViewModel TransactionListViewModelDesign { get; } = GetTransactionListViewModel();
    
    private static IServiceProvider GetProvider()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddAvalonServices();
        serviceCollection.AddSingleton<IDataService, MockDataService>();
        return serviceCollection.BuildServiceProvider();
    }
    
    private static SettingsViewModel GetSettingsViewModel()
    {
        SettingsViewModel viewModel = serviceProvider.GetRequiredService<SettingsViewModel>();
        return viewModel;
    }
    
    private static TravelListViewModel GetTravelListViewModel()
    {
        TravelListViewModel viewModel = serviceProvider.GetRequiredService<TravelListViewModel>();
        return viewModel;
    }
    
    private static TravelDetailViewModel GetTravelDetailViewModel()
    {
        SetSession();
        TravelDetailViewModel viewModel = serviceProvider.GetRequiredService<TravelDetailViewModel>();
        return viewModel;
    }
    
    private static TransactionListViewModel GetTransactionListViewModel()
    {
        SetSession();
        TransactionListViewModel viewModel = serviceProvider.GetRequiredService<TransactionListViewModel>();
        return viewModel;
    }
    
    private static void SetSession()
    {
        SessionService session = serviceProvider.GetRequiredService<SessionService>();
        session.CurrentTravel = session.GetAllTravels().First();
    }
}