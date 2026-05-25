using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using TripCostSplitter.Core.JsonExtensions;

namespace TripCostSplitter.Core.DataModels;

public partial class Transaction: ObservableObject
{
    public string TransactionId { get; init; } = "";
    
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
    public required partial ITransactionData TransactionData { get; set; }
    
    [ObservableProperty]
    public partial ObservableCollection<string> Images { get; set; } = [];
    
    [ObservableProperty]
    public partial IReadOnlyList<RecipientInfo> RecipientInfos { get; set; } = [];
    
    public Transaction Copy()
    {
        string serialized = JsonSerializer.Serialize(this);
        return JsonSerializer.Deserialize<Transaction>(serialized)!;
    }
}