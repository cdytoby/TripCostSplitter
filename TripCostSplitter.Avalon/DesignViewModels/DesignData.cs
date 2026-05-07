using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.AppBase.ViewModels;
using TripCostSplitter.AppBase.ViewModels.SplitViewModels;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.Avalon.DesignViewModels;

public static class DesignData
{
    private static IServiceProvider serviceProvider { get; } = GetProvider();
    
    public static SettingsViewModel ExampleSettingsViewModel { get; } = GetSettingsViewModel();
    
    public static TravelListViewModel TravelListViewModelDesign { get; } = GetTravelListViewModel();
    
    public static TravelDetailViewModel TravelDetailViewModelDesign { get; } = GetTravelDetailViewModel();
    
    public static TransactionListViewModel TransactionListViewModelDesign { get; } = GetTransactionListViewModel();
    
    public static PaymentDetailViewModel PaymentDetailViewModelDesign { get; } = GetPaymentDetailViewModel();
    
    public static SplitByPercentageViewModel SplitByPercentageViewModelDesign { get; } =
        GetSplitByPercentageViewModel();
    
    public static SplitByExactAmountViewModel SplitBySplitByExactAmountViewModelDesign { get; } =
        GetSplitByExactAmountViewModel();
    
    public static SplitByItemOwnershipViewModel SplitByItemOwnershipViewModelDesign { get; } =
        GetSplitByItemOwnershipViewModel();
    
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
    
    private static PaymentDetailViewModel GetPaymentDetailViewModel()
    {
        SetSession();
        PaymentDetailViewModel viewModel = serviceProvider.GetRequiredService<PaymentDetailViewModel>();
        return viewModel;
    }
    
    private static SplitByPercentageViewModel GetSplitByPercentageViewModel()
    {
        SetSession();
        SessionService session = serviceProvider.GetRequiredService<SessionService>();
        PaymentData paymentData = (session.CurrentTransaction!.TransactionData as PaymentData)!;
        paymentData.SplitData = new SplitByPercentage
        {
            PersonPortionDict = new Dictionary<string, decimal>()
            {
                { "1", 0.5m },
                { "2", 0.5m }
            }
        };
        SplitByPercentageViewModel viewModel = new();
        viewModel.Load(
            paymentData,
            session.CurrentTravel!.Participants,
            CurrencyService.GetCurrencyInfo("EUR")!);
        return viewModel;
    }
    
    private static SplitByExactAmountViewModel GetSplitByExactAmountViewModel()
    {
        SetSession();
        SessionService session = serviceProvider.GetRequiredService<SessionService>();
        PaymentData paymentData = (session.CurrentTransaction!.TransactionData as PaymentData)!;
        paymentData.SplitData = new SplitByExactAmount
        {
            PersonIdAmountDict = new Dictionary<string, decimal>()
            {
                { "1", 40m },
                { "2", 60m }
            }
        };
        SplitByExactAmountViewModel viewModel = new();
        viewModel.Load(
            paymentData,
            session.CurrentTravel!.Participants,
            CurrencyService.GetCurrencyInfo("EUR")!);
        return viewModel;
    }
    
    private static SplitByItemOwnershipViewModel GetSplitByItemOwnershipViewModel()
    {
        SetSession(1);
        SessionService session = serviceProvider.GetRequiredService<SessionService>();
        PaymentData paymentData = (session.CurrentTransaction!.TransactionData as PaymentData)!;
        paymentData.SplitData = new SplitByItemOwnership
        {
            OwnershipGroups = new()
            {
                {
                    "1", [
                        "Pasta",
                        "Apple juice",
                        "Chicken Wings",
                        "Steak",
                        "Tip"
                    ]
                },
                {
                    "2", [
                        "Pizza",
                        "Orange juice",
                        "Tiramisu",
                        "Tip"
                    ]
                }
            }
        };
        SplitByItemOwnershipViewModel viewModel = new();
        viewModel.Load(
            paymentData,
            session.CurrentTravel!.Participants,
            CurrencyService.GetCurrencyInfo("EUR")!);
        return viewModel;
    }
    
    private static void SetSession(int transactionIndex = 0)
    {
        SessionService session = serviceProvider.GetRequiredService<SessionService>();
        session.CurrentTravel = session.GetAllTravels().First();
        session.CurrentTransaction = session.GetAllTravels().First().Transactions[transactionIndex];
    }
}