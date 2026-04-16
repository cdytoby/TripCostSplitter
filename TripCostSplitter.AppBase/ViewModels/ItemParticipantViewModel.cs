using CommunityToolkit.Mvvm.ComponentModel;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.AppBase.ViewModels;

public partial class ItemParticipantViewModel : ObservableObject
{
    public Person Person { get; }
    
    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    public ItemParticipantViewModel(Person person, bool isSelected = false)
    {
        Person = person;
        IsSelected = isSelected;
    }
}
