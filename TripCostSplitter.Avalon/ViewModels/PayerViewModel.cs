using CommunityToolkit.Mvvm.ComponentModel;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Avalon.ViewModels;

public partial class PayerViewModel : ObservableObject
{
    public Person Person { get; }
    
    [ObservableProperty]
    public partial decimal Amount { get; set; }

    public PayerViewModel(Person person, decimal amount)
    {
        Person = person;
        Amount = amount;
    }
}
