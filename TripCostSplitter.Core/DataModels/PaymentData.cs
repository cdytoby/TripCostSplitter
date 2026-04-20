using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using TripCostSplitter.Core.JsonExtensions;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.Core.DataModels;

public partial class PaymentData: ObservableObject, ITransactionData
{
    [ObservableProperty]
    public required partial DateTime Date { get; set; }
    
    [JsonConverter(typeof(TimeZoneInfoConverter))]
    public required TimeZoneInfo DateTimeZone { get; set; }
    
    [ObservableProperty]
    public partial string? Description { get; set; }
    
    [ObservableProperty]
    public required partial string Currency { get; set; }
    
    [ObservableProperty]
    public partial decimal? ExchangeRateOverride { get; set; }
    
    [ObservableProperty]
    public required partial ObservableCollection<PayerInfo> PayerInfos { get; set; }
    
    [ObservableProperty]
    public required partial ObservableCollection<int> ParticipantIds { get; set; }
    
    [ObservableProperty]
    public required partial ObservableCollection<PurchaseItem> PurchaseItems { get; set; }
    
    [ObservableProperty]
    public partial ISplitData? SplitData { get; set; }
}