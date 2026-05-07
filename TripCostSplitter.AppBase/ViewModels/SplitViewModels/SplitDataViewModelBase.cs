using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.AppBase.ViewModels.SplitViewModels;

public abstract class SplitDataViewModelBase(IMessenger? messenger = null):
    ObservableRecipient(messenger ?? new WeakReferenceMessenger())
{
    public abstract ISplitData Save();
    
    public abstract void Load(
        PaymentData paymentData, IReadOnlyCollection<Person> travelParticipants, CurrencyModel useCurrency);
}