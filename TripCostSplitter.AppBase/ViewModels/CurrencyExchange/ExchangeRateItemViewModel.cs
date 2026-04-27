using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.AppBase.ViewModels.CurrencyExchange;

public partial class ExchangeRateItemViewModel(IMessenger messenger): ObservableRecipient(messenger)
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Valid))]
    [NotifyPropertyChangedFor(nameof(IsLeftCurrencyValid), nameof(IsRightCurrencyValid))]
    [NotifyPropertyChangedRecipients]
    public partial CurrencyModel? LeftCurrency { get; set; }
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Valid))]
    [NotifyPropertyChangedFor(nameof(IsLeftCurrencyValid), nameof(IsRightCurrencyValid))]
    [NotifyPropertyChangedRecipients]
    public partial CurrencyModel? RightCurrency { get; set; }
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Valid))]
    public partial decimal LeftToRightRate { get; set; }
    
    [ObservableProperty]
    public partial bool Duplicate { get; set; }
    
    public decimal RightToLeftRate => 1.0m / LeftToRightRate;
    public bool IsLeftCurrencyValid => LeftCurrency != null && LeftCurrency != RightCurrency;
    public bool IsRightCurrencyValid => RightCurrency != null && LeftCurrency != RightCurrency;
    public bool Valid => Validate();
    
    public void Load(CurrencyExchangeRateModel model, CurrencyService currencyService)
    {
        LeftCurrency = currencyService.GetCurrencyInfo(model.fromCurrency);
        RightCurrency = currencyService.GetCurrencyInfo(model.toCurrency);
        LeftToRightRate = model.rate;
    }
    
    public CurrencyExchangeRateModel? Save()
    {
        return Validate() ?
            new CurrencyExchangeRateModel(LeftCurrency!.Code, RightCurrency!.Code, LeftToRightRate) :
            null;
    }
    
    private bool Validate()
    {
        return IsLeftCurrencyValid &&
            IsRightCurrencyValid &&
            LeftToRightRate > 0 &&
            RightToLeftRate > 0;
    }
    
    public bool IsDuplicate(ExchangeRateItemViewModel otherVm)
    {
        if (LeftCurrency == null ||
            RightCurrency == null ||
            otherVm.LeftCurrency == null ||
            otherVm.RightCurrency == null)
            return false;
        
        if (LeftCurrency.Equals(otherVm.LeftCurrency) && RightCurrency.Equals(otherVm.RightCurrency))
            return true;
        if (LeftCurrency.Equals(otherVm.RightCurrency) && RightCurrency.Equals(otherVm.LeftCurrency))
            return true;
        return false;
    }
}