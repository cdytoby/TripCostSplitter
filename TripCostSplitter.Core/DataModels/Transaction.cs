using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TripCostSplitter.Core.DataModels;

public partial class Transaction: ObservableObject
{
    public string TransactionId { get; init; } = "";
    
    [ObservableProperty]
    public required partial string Currency { get; set; }
    
    [ObservableProperty]
    public partial decimal? ExchangeRateOverride { get; set; }
    
    [ObservableProperty]
    public required partial ITransactionData TransactionData { get; set; }
    
    [ObservableProperty]
    public partial IReadOnlyCollection<RecipientInfo> RecipientInfos { get; set; } = [];
}