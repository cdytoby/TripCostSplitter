using CommunityToolkit.Mvvm.ComponentModel;

namespace TripCostSplitter.Core.DataModels;

public partial class PayerInfo: ObservableObject
{
    public int PayerId { get; }
    
    [ObservableProperty]
    public partial decimal Amount { get; set; }
    
    public PayerInfo(int payerId, decimal amount = 0)
    {
        PayerId = payerId;
        Amount = amount;
    }
}