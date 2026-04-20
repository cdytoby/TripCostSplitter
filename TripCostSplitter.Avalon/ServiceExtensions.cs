using Microsoft.Extensions.DependencyInjection;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.AppBase.ViewModels;
using TripCostSplitter.Avalon.Services;
using TripCostSplitter.Avalon.Views;
using TripCostSplitter.Core;
using TripCostSplitter.Core.Services;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.Avalon;

public static class ServiceExtensions
{
    public static void AddTripCostSplitterServices(this IServiceCollection services)
    {
        // ViewModels
        services.AddTransient<TravelListViewModel>();
        services.AddTransient<TravelDetailViewModel>();
        services.AddTransient<PaymentDetailViewModel>();
        services.AddTransient<TransactionListViewModel>();
        services.AddTransient<DebtsViewModel>();
        
        // Views
        services.AddTransient<MainView>();
        services.AddTransient<TravelListView>();
        services.AddTransient<TravelView>();
        services.AddTransient<PaymentDetailView>();
        services.AddTransient<TravelDetailsTab>();
        services.AddTransient<TravelTransactionsTab>();
        services.AddTransient<TravelDebtsTab>();
        
        // Services
        services.AddSingleton<IAppDispatcherService, AvalonDispatcherService>();
        services.AddSingleton<INavigationService, AvalonNavigationService>();
        services.AddSingleton<CurrencyService>();
        services.AddSingleton<AccessManager>();
        services.AddSingleton<SessionService>();
        services.AddSingleton<SplitDataViewModelService>();
        
        // Split Calculators
        services.AddSingleton<ISplitCalculator, SplitEvenlyCalculator>();
        services.AddSingleton<ISplitCalculator, SplitByExactAmountCalculator>();
        services.AddSingleton<ISplitCalculator, SplitByPercentageCalculator>();
        services.AddSingleton<ISplitCalculator, SplitByItemOwnershipCalculator>();
    }
}