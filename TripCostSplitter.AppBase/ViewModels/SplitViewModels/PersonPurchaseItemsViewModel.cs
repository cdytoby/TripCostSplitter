using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.AppBase.ViewModels.SplitViewModels;

public partial class PersonPurchaseItemsViewModel: ObservableObject
{
    [ObservableProperty]
    public partial Person Person { get; set; }
    
    [ObservableProperty]
    public partial ObservableCollection<string> OwnedItems { get; set; } = new();
    
    [ObservableProperty]
    public partial ObservableCollection<string> AvailableToAdd { get; set; } = new();
    
    public IRelayCommand AddItemCommand { get; }
    public IRelayCommand RemoveItemCommand { get; }
    
    private readonly List<string> allItems = new();
    
    public PersonPurchaseItemsViewModel(Person person)
    {
        Person = person;
        
        AddItemCommand = new RelayCommand<string>(AddItem);
        RemoveItemCommand = new RelayCommand<string>(RemoveItem);
    }
    
    public PersonPurchaseItemsViewModel(
        Person person,
        IEnumerable<string> _allItems,
        IEnumerable<string>? initialOwnedItems = null): this(person)
    {
        allItems.AddRange(_allItems);
        
        HashSet<string> initial = initialOwnedItems == null ?
            [] :
            [..initialOwnedItems];
        
        foreach (string item in allItems.Where(item => initial.Contains(item)))
        {
            OwnedItems.Add(item);
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
