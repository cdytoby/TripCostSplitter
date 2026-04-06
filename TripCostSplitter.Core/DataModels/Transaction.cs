using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TripCostSplitter.Core.DataModels;

public partial class Transaction: ObservableObject
{
    [ObservableProperty]
    public required partial ITransactionData TransactionData { get; set; }
    
    [ObservableProperty]
    public required partial IList<RecipientInfo> RecipientInfos { get; set; }
}