using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.AppBase.ViewModels.SplitViewModels;

public partial class PurchaseItemOwnerViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string ItemName { get; set; }

    [ObservableProperty]
    public partial Person Owner { get; set; }

    public PurchaseItemOwnerViewModel(string itemName, Person owner)
    {
        ItemName = itemName;
        Owner = owner;
    }
}
