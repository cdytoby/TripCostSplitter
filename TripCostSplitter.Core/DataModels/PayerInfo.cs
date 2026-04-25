using CommunityToolkit.Mvvm.ComponentModel;

namespace TripCostSplitter.Core.DataModels;

public partial class PayerInfo: ObservableObject
{
    public string PayerId { get; }
    
    [ObservableProperty]
    public partial decimal Amount { get; set; }
    
    public PayerInfo(string payerId, decimal amount = 0)
    {
        PayerId = payerId;
        Amount = amount;
    }
}