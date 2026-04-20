using CommunityToolkit.Mvvm.ComponentModel;

namespace TripCostSplitter.Core.DataModels;

public partial class PurchaseItem: ObservableObject
{
    [ObservableProperty]
    public partial string Item { get; set; }
    
    [ObservableProperty]
    public partial decimal Price { get; set; }
    
    public PurchaseItem(string item, decimal price)
    {
        Item = item;
        Price = price;
    }
}