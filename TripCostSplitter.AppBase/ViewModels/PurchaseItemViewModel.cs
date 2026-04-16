using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.AppBase.ViewModels;

public partial class PurchaseItemViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string ItemName { get; set; }

    [ObservableProperty]
    public partial decimal Amount { get; set; }

    public PurchaseItem Item => new(ItemName, Amount);

    public ObservableCollection<ItemParticipantViewModel> Participants { get; } = [];

    public PurchaseItemViewModel(PurchaseItem item, IEnumerable<Person> allParticipants)
    {
        ItemName = item.Item;
        Amount = item.Amount;
        foreach (Person person in allParticipants)
        {
            Participants.Add(new ItemParticipantViewModel(person));
        }
    }
}
