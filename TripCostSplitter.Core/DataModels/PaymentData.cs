using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.Core.DataModels;

public partial class PaymentData: ObservableObject, ITransactionData
{
    public string TransactionType => "Payment";

    [ObservableProperty]
    public required partial DateTime Date { get; set; }
    
    [ObservableProperty]
    public partial string? Description { get; set; }
    
    [ObservableProperty]
    public required partial string Currency { get; set; }
    
    [ObservableProperty]
    public partial decimal? ExchangeRateOverride { get; set; }
    
    [ObservableProperty]
    public required partial ObservableCollection<PayerInfo> PayerInfos { get; set; }
    
    [ObservableProperty]
    public required partial ObservableCollection<Person> Participants { get; set; }
    
    [ObservableProperty]
    public required partial ObservableCollection<PurchaseItem> PurchaseItems { get; set; }
    
    [ObservableProperty]
    public partial ISplitData? SplitData { get; set; }
}