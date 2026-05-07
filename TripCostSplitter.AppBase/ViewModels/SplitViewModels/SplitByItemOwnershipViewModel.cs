using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using TripCostSplitter.AppBase.Messages;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.AppBase.ViewModels.SplitViewModels;

public partial class SplitByItemOwnershipViewModel: SplitDataViewModelBase
{
    [ObservableProperty]
    public partial IReadOnlyCollection<string> Items { get; private set; } = [];
    
    [ObservableProperty]
    public partial IReadOnlyCollection<Person> Participants { get; private set; } = [];
    
    [ObservableProperty]
    public partial ObservableCollection<PersonPurchaseItemsViewModel> PersonItemsViewModels { get; private set; } = [];
    
    [ObservableProperty]
    public partial ObservableCollection<string> RemainingItems { get; private set; } = [];
    
    [ObservableProperty]
    public partial bool HasError { get; set; }
    
    private CurrencyModel? currency;
    private PaymentData? loadedData;
    
    public SplitByItemOwnershipViewModel(IMessenger? messenger = null): base(messenger)
    {
        Messenger.Register<PropertyChangedMessage<CurrencyModel>>(this, OnCurrencyChanged);
        Messenger.Register<PaymentItemsChangedMessage>(this, OnPurchasedItemsChanged);
        Messenger.Register<OwnedItemsChangedMessage>(this, OnOwnedItemsChanged);
    }
    
    public override void Load(
        PaymentData paymentData, IReadOnlyCollection<Person> travelParticipants, CurrencyModel useCurrency)
    {
        if (paymentData.SplitData is not SplitByItemOwnership itemOwnershipData)
            return;
        
        loadedData = paymentData;
        Items = paymentData.PurchaseItems.Select(pi => pi.ItemName).ToList();
        Participants = travelParticipants;
        currency = useCurrency;
        
        foreach (Person participant in Participants)
        {
            List<string> ownedItems = [];
            if (itemOwnershipData.OwnershipGroups.TryGetValue(participant.Id, out List<string>? items))
            {
                ownedItems.AddRange(items.Where(i => Items.Contains(i)));
            }
            
            PersonItemsViewModels.Add(new PersonPurchaseItemsViewModel(Messenger, participant, Items, ownedItems));
        }
        
        Refresh();
    }
    
    private void SyncItemsFromPaymentData()
    {
        if (loadedData == null)
            return;
        Items = loadedData.PurchaseItems.Select(pi => pi.ItemName).ToList();
        foreach (PersonPurchaseItemsViewModel vm in PersonItemsViewModels)
        {
            vm.UpdateAllItems(Items);
        }
    }
    
    private void OnCurrencyChanged(object recipient, PropertyChangedMessage<CurrencyModel> message)
    {
        currency = message.NewValue;
        Refresh();
    }
    
    private void OnPurchasedItemsChanged(object recipient, PaymentItemsChangedMessage message)
    {
        SyncItemsFromPaymentData();
        Refresh();
    }
    
    private void OnOwnedItemsChanged(object recipient, OwnedItemsChangedMessage message)
    {
        Refresh();
    }
    
    private void Refresh()
    {
        if (loadedData == null)
            return;
        
        UpdateAmountsPaid();
        UpdateRemainingItems();
        UpdateError();
    }
    
    private void UpdateAmountsPaid()
    {
        if (loadedData == null)
            return;
        
        Dictionary<string, int> unitCountDict = [];
        foreach (PersonPurchaseItemsViewModel vm in PersonItemsViewModels)
        {
            foreach (string vmOwnedItem in vm.OwnedItems)
            {
                unitCountDict.TryAdd(vmOwnedItem, 0);
                unitCountDict[vmOwnedItem]++;
            }
        }
        
        foreach (PersonPurchaseItemsViewModel vm in PersonItemsViewModels)
        {
            decimal amount = 0;
            foreach (string ownedItem in vm.OwnedItems)
            {
                decimal unitPrice = loadedData.PurchaseItems.FirstOrDefault(p => p.ItemName.Equals(ownedItem))?.Price ?? 0;
                amount += unitPrice / unitCountDict[ownedItem];
            }
            
            vm.AmountPaid = amount;
            vm.AmountPaidFormatted = amount.ToString("C2", CurrencyService.GetNumberFormat(currency!.Code));
        }
    }
    
    private void UpdateRemainingItems()
    {
        HashSet<string> assigned = [];
        foreach (PersonPurchaseItemsViewModel vm in PersonItemsViewModels)
        {
            foreach (string item in vm.OwnedItems)
            {
                assigned.Add(item);
            }
        }
        
        RemainingItems.Clear();
        foreach (string item in Items)
        {
            if (!assigned.Contains(item))
            {
                RemainingItems.Add(item);
            }
        }
    }
    
    private void UpdateError()
    {
        HasError = RemainingItems.Count > 0;
    }
    
    public override ISplitData Save()
    {
        SplitByItemOwnership splitData = new();
        
        foreach (PersonPurchaseItemsViewModel viewModel in PersonItemsViewModels)
        {
            if (viewModel.OwnedItems.Count > 0)
                splitData.OwnershipGroups[viewModel.Person.Id] = viewModel.OwnedItems.ToList();
        }
        
        Validate(splitData);
        
        return splitData;
    }
    
    private void Validate(SplitByItemOwnership splitData)
    {
        PersonPurchaseItemsViewModel? leastPaid = PersonItemsViewModels
            .OrderBy(vm => vm.AmountPaid)
            .FirstOrDefault();
        
        if (leastPaid != null && RemainingItems.Count > 0)
        {
            splitData.EnsurePerson(leastPaid.Person.Id);
            splitData.OwnershipGroups[leastPaid.Person.Id].AddRange(RemainingItems);
        }
    }
}