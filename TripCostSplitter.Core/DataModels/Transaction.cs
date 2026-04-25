using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TripCostSplitter.Core.DataModels;

public partial class Transaction: ObservableObject
{
    public string TransactionId { get; init; } = "";
    
    [ObservableProperty]
    public required partial ITransactionData TransactionData { get; set; }
    
    [ObservableProperty]
    public required partial ObservableCollection<RecipientInfo> RecipientInfos { get; set; }
}