using CommunityToolkit.Mvvm.ComponentModel;

namespace TripCostSplitter.Core.DataModels;

public partial class PurchaseItem: ObservableObject
{
    [ObservableProperty]
    public partial string ItemName { get; set; }
    
    [ObservableProperty]
    public partial decimal Price { get; set; }
    
    public PurchaseItem(string itemName, decimal price)
    {
        ItemName = itemName;
        Price = price;
    }
}