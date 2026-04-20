using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.AppBase.ViewModels.SplitViewModels;

public abstract class SplitDataViewModelBase : ObservableObject
{
    public abstract ISplitData Save();
    
    public abstract void Load(
        ISplitData splitData, IReadOnlyCollection<Person> travelParticipants, PaymentData paymentData);
}