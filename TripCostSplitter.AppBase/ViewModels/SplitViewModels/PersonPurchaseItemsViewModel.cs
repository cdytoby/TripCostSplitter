using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TripCostSplitter.AppBase.Messages;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.AppBase.ViewModels.SplitViewModels;

public partial class PersonPurchaseItemsViewModel: ObservableRecipient
{
    [ObservableProperty]
    public partial Person Person { get; set; }
    
    [ObservableProperty]
    public partial ObservableCollection<string> OwnedItems { get; set; } = [];
    
    [ObservableProperty]
    public partial ObservableCollection<string> AvailableToAdd { get; set; } = [];
    
    [ObservableProperty]
    public partial string AmountPaidFormatted { get; set; } = string.Empty;
    
    public decimal AmountPaid { get; set; }
    public IRelayCommand AddItemCommand { get; }
    public IRelayCommand RemoveItemCommand { get; }
    
    private readonly List<string> allItems = [];
    
    public PersonPurchaseItemsViewModel(
        IMessenger messenger,
        Person person,
        IEnumerable<string> _allItems,
        IEnumerable<string>? initialOwnedItems = null): base(messenger)
    {
        AddItemCommand = new RelayCommand<string>(AddItem);
        RemoveItemCommand = new RelayCommand<string>(RemoveItem);
        
        Person = person;
        allItems.AddRange(_allItems);
        
        HashSet<string> initial = initialOwnedItems == null ?
            [] :
            [..initialOwnedItems];
        
        foreach (string item in allItems.Where(item => initial.Contains(item)))
        {
            OwnedItems!.Add(item);
        }
        
        RefreshAvailableToAdd();
    }
    
    public void UpdateAllItems(IEnumerable<string> newAllItems)
    {
        allItems.Clear();
        allItems.AddRange(newAllItems);
        
        for (int i = OwnedItems.Count - 1; i >= 0; i--)
        {
            if (!allItems.Contains(OwnedItems[i]))
                OwnedItems.RemoveAt(i);
        }
        
        RefreshAvailableToAdd();
    }
    
    private void AddItem(string? item)
    {
        if (item == null)
            return;
        if (OwnedItems.Any(i => i == item))
            return;
        OwnedItems.Add(item);
        Messenger.Send(new OwnedItemsChangedMessage());
        RefreshAvailableToAdd();
    }
    
    private void RemoveItem(string? item)
    {
        if (item == null)
            return;
        string? existing = OwnedItems.FirstOrDefault(i => i == item);
        if (existing == null)
            return;
        OwnedItems.Remove(existing);
        Messenger.Send(new OwnedItemsChangedMessage());
        RefreshAvailableToAdd();
    }
    
    private void RefreshAvailableToAdd()
    {
        HashSet<string> owned = new(OwnedItems);
        AvailableToAdd.Clear();
        foreach (string item in allItems.Where(item => !owned.Contains(item)))
        {
            AvailableToAdd.Add(item);
        }
    }
}
