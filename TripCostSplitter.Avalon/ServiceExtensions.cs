using Microsoft.Extensions.DependencyInjection;
using TripCostSplitter.AppBase;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.Avalon.Services;
using TripCostSplitter.Avalon.Views;

namespace TripCostSplitter.Avalon;

public static class ServiceExtensions
{
    public static void AddAvalonServices(this IServiceCollection services)
    {
        services.AddAppServices();
        
        // Views
        services.AddTransient<MainView>();
        services.AddTransient<TravelListView>();
        services.AddTransient<TravelView>();
        services.AddTransient<PaymentDetailView>();
        services.AddTransient<TravelDetailsView>();
        services.AddTransient<TravelTransactionsView>();
        services.AddTransient<TravelDebtsView>();
        
        // Services
        services.AddSingleton<IAppDispatcherService, AvalonDispatcherService>();
        services.AddSingleton<INavigationService, AvalonNavigationService>();
    }
}