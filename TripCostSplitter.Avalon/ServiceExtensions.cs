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
        services.AddTransient<TransactionDetailViewModel>();
        services.AddTransient<DebtResultViewModel>();

        // Views
        services.AddTransient<MainView>();
        services.AddTransient<TravelListView>();
        services.AddTransient<TravelDetailView>();
        services.AddTransient<TransactionDetailView>();
        services.AddTransient<DebtResultView>();

        // Services
        services.AddSingleton<IAppDispatcherService, AvalonDispatcherService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<CurrencyService>();
        services.AddSingleton<AccessManager>();
        services.AddSingleton<SessionService>();

        // Split Calculators
        services.AddSingleton<ISplitCalculator, SplitEvenlyCalculator>();
        services.AddSingleton<ISplitCalculator, SplitByExactAmountCalculator>();
        services.AddSingleton<ISplitCalculator, SplitByPercentageCalculator>();
        services.AddSingleton<ISplitCalculator, SplitByItemOwnershipCalculator>();
    }
}
