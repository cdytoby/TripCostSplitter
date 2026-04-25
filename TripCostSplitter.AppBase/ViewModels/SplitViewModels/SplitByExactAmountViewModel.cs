using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.AppBase.ViewModels.SplitViewModels;

public partial class SplitByExactAmountViewModel : SplitDataViewModelBase
{
    [ObservableProperty]
    public partial ObservableCollection<SplitParticipantViewModel> SplitParticipants { get; set; } = [];
    
    public override void Load(ISplitData splitData, IReadOnlyCollection<Person> travelParticipants, PaymentData paymentData)
    {
        if (splitData is SplitByExactAmount exactAmountData)
        {
            foreach (KeyValuePair<string, decimal> kvp in exactAmountData.PersonIdAmountDict)
            {
                SplitParticipants.Add(
                    new SplitParticipantViewModel(
                        travelParticipants.Single(p => p.Id.Equals(kvp.Key)),
                        kvp.Value));
            }
        }
    }
    
    //todo validate
    public override ISplitData Save()
    {
        SplitByExactAmount exact = new();
        foreach (SplitParticipantViewModel sp in SplitParticipants)
        {
            exact.PersonIdAmountDict[sp.Person.Id] = sp.Value;
        }
        
        return exact;
    }
}