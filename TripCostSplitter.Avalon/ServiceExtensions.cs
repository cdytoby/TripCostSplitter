using Microsoft.Extensions.DependencyInjection;
using TripCostSplitter.Avalon.ViewModels;
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
        services.AddSingleton<MainViewModel>();
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
        services.AddSingleton<CurrencyService>();

        // Split Calculators
        services.AddSingleton<ISplitCalculator, SplitEvenlyCalculator>();
        services.AddSingleton<ISplitCalculator, SplitByExactAmountCalculator>();
        services.AddSingleton<ISplitCalculator, SplitByPercentageCalculator>();
        services.AddSingleton<ISplitCalculator, SplitByItemOwnershipCalculator>();
    }
}
