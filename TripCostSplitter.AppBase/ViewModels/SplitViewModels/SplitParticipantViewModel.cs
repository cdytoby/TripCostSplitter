using CommunityToolkit.Mvvm.ComponentModel;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.AppBase.ViewModels.SplitViewModels;

public partial class SplitParticipantViewModel : ObservableObject
{
    public Person Person { get; }

    [ObservableProperty]
    public partial decimal Value { get; set; }

    public SplitParticipantViewModel(Person person, decimal value = 0)
    {
        Person = person;
        Value = value;
    }
}
