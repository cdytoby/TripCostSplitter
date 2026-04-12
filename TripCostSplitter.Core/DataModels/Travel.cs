using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TripCostSplitter.Core.DataModels;

public partial class Travel: ObservableObject
{
    public int TravelId { get; init; }
    
    [ObservableProperty]
    public required partial string Name { get; set; }
    
    [ObservableProperty]
    public required partial string CalculateCurrency { get; set; }
    
    [ObservableProperty]
    public required partial ObservableCollection<Transaction> Transactions { get; set; }
}