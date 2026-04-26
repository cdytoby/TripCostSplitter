using CommunityToolkit.Mvvm.ComponentModel;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.AppBase.ViewModels.CurrencyExchange;

public partial class ExchangeRateItemViewModel: ObservableObject
{
    [ObservableProperty]
    public partial CurrencyModel? LeftCurrency { get; set; }
    
    [ObservableProperty]
    public partial CurrencyModel? RightCurrency { get; set; }
    
    [ObservableProperty]
    public partial decimal LeftToRightRate { get; set; }
    
    [ObservableProperty]
    public partial decimal RightToLeftRate { get; set; }
    
    partial void OnLeftToRightRateChanged(decimal value)
    {
        RightToLeftRate = 1.0m / value;
    }
    
    public static ExchangeRateItemViewModel? Load(CurrencyExchangeRateModel model, CurrencyService currencyService)
    {
        CurrencyModel? leftCurrency = currencyService.GetCurrencyInfo(model.fromCurrency);
        CurrencyModel? rightCurrency = currencyService.GetCurrencyInfo(model.toCurrency);
        if (leftCurrency == null || rightCurrency == null)
            return null;
        
        return new ExchangeRateItemViewModel
        {
            LeftCurrency = leftCurrency,
            RightCurrency = rightCurrency,
            LeftToRightRate = model.rate
        };
    }
    
    public CurrencyExchangeRateModel? Save()
    {
        if (LeftCurrency == null || RightCurrency == null)
            return null;
        return new CurrencyExchangeRateModel(LeftCurrency.Code, RightCurrency.Code, LeftToRightRate);
    }
}