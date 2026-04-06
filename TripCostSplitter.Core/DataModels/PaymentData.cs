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
    public required partial IList<Person> Participants { get; set; }
    
    [ObservableProperty]
    public required partial IList<PurchaseItem> PurchaseItems { get; set; }
    
    [ObservableProperty]
    public required partial IList<PayerInfo> PayerInfos { get; set; }
    
    [ObservableProperty]
    public partial ISplitData? SplitData { get; set; }
}