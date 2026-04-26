using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TripCostSplitter.Core.DataModels;

public partial class SettingsDataModel: ObservableObject
{
    [ObservableProperty]
    public partial string? ActiveTravelId { get; set; }
    
    public ICollection<CurrencyExchangeRateModel> CachedExchangeRates { get; set; } = [];
    
    [ObservableProperty]
    public partial string? DefaultCurrency { get; set; }
}