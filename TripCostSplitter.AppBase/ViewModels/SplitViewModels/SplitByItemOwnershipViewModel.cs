using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.AppBase.ViewModels.SplitViewModels;

public partial class SplitByItemOwnershipViewModel(IMessenger? messenger = null): SplitDataViewModelBase(messenger)
{
    [ObservableProperty]
    public partial IReadOnlyCollection<string> Items { get; private set; } = [];
    
    [ObservableProperty]
    public partial IReadOnlyCollection<Person> Participants { get; private set; } = [];
    
    [ObservableProperty]
    public partial ObservableCollection<PersonPurchaseItemsViewModel> PersonItemsViewModels { get; private set; } = [];
    
    public override void Load(
        PaymentData paymentData, IReadOnlyCollection<Person> travelParticipants, CurrencyModel useCurrency)
    {
        if (paymentData.SplitData is not SplitByItemOwnership itemOwnershipData)
            return;
        
        Items = paymentData.PurchaseItems.Select(pi => pi.Item).ToList();
        Participants = travelParticipants;
        
        foreach (Person participant in Participants)
        {
            List<string> ownedItems = [];
            if (itemOwnershipData.OwnershipGroups.TryGetValue(participant.Id, out List<string>? items) &&
                items != null)
            {
                ownedItems.AddRange(items.Where(i => Items.Contains(i)));
            }
            
            PersonItemsViewModels.Add(
                new PersonPurchaseItemsViewModel(participant, Items, ownedItems));
        }
    }
    
    //todo validate
    public override ISplitData Save()
    {
        SplitByItemOwnership splitData = new();
        foreach (PersonPurchaseItemsViewModel viewModel in PersonItemsViewModels)
        {
            List<string> ownedItems = viewModel.OwnedItems.ToList();
            splitData.OwnershipGroups[viewModel.Person.Id] = ownedItems;
        }
        
        return splitData;
    }
}
