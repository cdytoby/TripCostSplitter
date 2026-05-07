using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.Core.DataModels;

public partial class PaymentData: ObservableObject, ITransactionData
{
    [ObservableProperty]
    public required partial ObservableCollection<PayerInfo> PayerInfos { get; set; }
    
    [ObservableProperty]
    public required partial ObservableCollection<string> ParticipantIds { get; set; }
    
    [ObservableProperty]
    public required partial ObservableCollection<PurchaseItem> PurchaseItems { get; set; }
    
    [ObservableProperty]
    public partial ISplitData? SplitData { get; set; }
}