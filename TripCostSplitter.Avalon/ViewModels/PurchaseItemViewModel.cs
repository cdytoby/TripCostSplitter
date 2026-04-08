using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Avalon.ViewModels;

public partial class PurchaseItemViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string ItemName { get; set; }

    [ObservableProperty]
    public partial decimal Amount { get; set; }

    public PurchaseItem Item => new PurchaseItem(ItemName, Amount);

    public ObservableCollection<ItemParticipantViewModel> Participants { get; } = new();

    public PurchaseItemViewModel(PurchaseItem item, IEnumerable<Person> allParticipants)
    {
        ItemName = item.Item;
        Amount = item.Amount;
        foreach (var person in allParticipants)
        {
            Participants.Add(new ItemParticipantViewModel(person));
        }
    }
}
