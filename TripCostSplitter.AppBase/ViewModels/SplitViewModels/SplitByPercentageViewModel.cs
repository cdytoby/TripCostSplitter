using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.AppBase.ViewModels.SplitViewModels;

public partial class SplitByPercentageViewModel: SplitDataViewModelBase
{
    [ObservableProperty]
    public partial ObservableCollection<SplitParticipantViewModel> SplitParticipants { get; set; } = [];
    
    private bool splitEvenStrictly;
    
    public override void Load(ISplitData splitData, IReadOnlyCollection<Person> travelParticipants, PaymentData paymentData)
    {
        if (splitData is SplitByPercentage percentageData)
        {
            foreach (KeyValuePair<string, decimal> kvp in percentageData.PersonPercentageDict)
            {
                SplitParticipants.Add(
                    new SplitParticipantViewModel(
                        travelParticipants.Single(p => p.Id.Equals(kvp.Key)),
                        kvp.Value));
            }
        }
        else if (splitData is SplitEvenly evenlyData)
        {
            if (evenlyData.SplitParticipants.Count == 0)
            {
                evenlyData.SplitParticipants.AddRange(travelParticipants.Select(p => p.Id));
            }
            foreach (string personId in evenlyData.SplitParticipants)
            {
                SplitParticipants.Add(
                    new SplitParticipantViewModel(
                        travelParticipants.Single(p => p.Id.Equals(personId)),
                        100m / evenlyData.SplitParticipants.Count));
            }
        }
        
        splitEvenStrictly = true;
    }
    
    //todo validate
    public override ISplitData Save()
    {
        if (!splitEvenStrictly)
        {
            SplitByPercentage splitByPercentage = new();
            foreach (SplitParticipantViewModel sp in SplitParticipants)
            {
                splitByPercentage.PersonPercentageDict[sp.Person.Id] = sp.Value;
            }
            
            return splitByPercentage;
        }
        else
        {
            SplitEvenly splitEvenly = new();
            splitEvenly.SplitParticipants.AddRange(SplitParticipants.Select(vm => vm.Person.Id));
            return splitEvenly;
        }
    }
}