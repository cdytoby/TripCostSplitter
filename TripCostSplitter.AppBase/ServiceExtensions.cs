using Microsoft.Extensions.DependencyInjection;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.AppBase.ViewModels;
using TripCostSplitter.Core.Services;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.AppBase;

public static class ServiceExtensions
{
    public static void AddAppServices(this IServiceCollection services)
    {
        // ViewModels
        services.AddTransient<TravelListViewModel>();
        services.AddTransient<TravelDetailViewModel>();
        services.AddTransient<PaymentDetailViewModel>();
        services.AddTransient<TransactionListViewModel>();
        services.AddTransient<DebtsViewModel>();
        services.AddTransient<SettingsViewModel>();
        
        // Services
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