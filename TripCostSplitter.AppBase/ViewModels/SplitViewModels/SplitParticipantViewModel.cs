using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.AppBase.ViewModels.SplitViewModels;

public partial class SplitParticipantViewModel: ObservableRecipient
{
    public Person Person { get; }
    
    [ObservableProperty]
    [NotifyPropertyChangedRecipients]
    public partial bool Enable { get; set; } = true;
    
    [ObservableProperty]
    [NotifyPropertyChangedRecipients]
    public partial decimal Value { get; set; }
    
    public SplitParticipantViewModel(IMessenger messenger, Person person, decimal value = 0): base(messenger)
    {
        Person = person;
        Value = value;
    }
}