using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.AppBase.ViewModels.SplitViewModels;

public partial class SplitByItemOwnershipViewModel: SplitDataViewModelBase
{
    public IReadOnlyCollection<string> Items { get; private set; } = [];
    public IReadOnlyCollection<Person> Participants { get; private set; } = [];
    
    [ObservableProperty]
    public partial ObservableCollection<PurchaseItemOwnerViewModel> ItemOwnerViewModels { get; private set; } = [];
    
    public override void Load(ISplitData splitData, IReadOnlyCollection<Person> travelParticipants, PaymentData paymentData)
    {
        if (splitData is SplitByItemOwnership itemOwnershipData)
        {
            Items = paymentData.PurchaseItems.Select(pi => pi.Item).ToList();
            Participants = travelParticipants;
            foreach (KeyValuePair<string, List<string>?> kvp in itemOwnershipData.OwnershipGroups)
            {
                if (kvp.Value == null)
                    continue;
                foreach (string itemName in kvp.Value)
                {
                    PurchaseItemOwnerViewModel itemVm =
                        new(itemName, travelParticipants.Single(p => p.Id.Equals(kvp.Key)));
                    ItemOwnerViewModels.Add(itemVm);
                }
            }
        }
    }
    
    //todo validate
    public override ISplitData Save()
    {
        SplitByItemOwnership splitData = new();
        foreach (PurchaseItemOwnerViewModel viewModel in ItemOwnerViewModels)
        {
            if (!splitData.OwnershipGroups.ContainsKey(viewModel.Owner.Id) ||
                splitData.OwnershipGroups[viewModel.Owner.Id] == null)
            {
                splitData.OwnershipGroups[viewModel.Owner.Id] = new List<string>();
            }
            
            splitData.OwnershipGroups[viewModel.Owner.Id]!.Add(viewModel.ItemName);
        }
        
        return splitData;
    }
}