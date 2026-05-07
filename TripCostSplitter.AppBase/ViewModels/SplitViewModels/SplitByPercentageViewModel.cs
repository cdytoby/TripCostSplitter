using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using TripCostSplitter.AppBase.Messages;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.AppBase.ViewModels.SplitViewModels;

public partial class SplitByPercentageViewModel: SplitDataViewModelBase
{
    [ObservableProperty]
    public partial ObservableCollection<SplitParticipantViewModel> SplitParticipants { get; set; } = [];
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanDisableParticipant))]
    public partial bool CanCustomSplit { get; set; } = true;
    
    [ObservableProperty]
    public partial bool HasError { get; set; } = false;
    
    [ObservableProperty]
    public partial decimal TotalPrice { get; set; } = 0;
    
    [ObservableProperty]
    public partial CurrencyModel? Currency { get; set; }
    
    public bool CanDisableParticipant => !CanCustomSplit;
    
    private bool isLoaded = false;
    
    public SplitByPercentageViewModel(IMessenger? messenger = null): base(messenger)
    {
        Messenger.Register<PropertyChangedMessage<CurrencyModel>>(this, CurrencyChanged);
        Messenger.Register<PropertyChangedMessage<bool>>(this, ParticipantEnableChanged);
        Messenger.Register<PropertyChangedMessage<decimal>>(this, ValueChanged);
        Messenger.Register<PaymentTotalValueChangedMessage>(this, TotalValueChanged);
    }
    
    public override void Load(
        PaymentData paymentData, IReadOnlyCollection<Person> travelParticipants, CurrencyModel useCurrency)
    {
        if (paymentData.SplitData is SplitByPercentage percentageData)
        {
            LoadPercentageSplit(percentageData, travelParticipants);
        }
        else if (paymentData.SplitData is SplitEvenly evenlyData)
        {
            LoadEvenSplit(evenlyData, travelParticipants);
        }
        
        TotalPrice = paymentData.PayerInfos.Sum(pi => pi.Amount);
        Currency = useCurrency;
        isLoaded = true;
    }
    
    private void LoadPercentageSplit(SplitByPercentage percentageData, IReadOnlyCollection<Person> travelParticipants)
    {
        foreach (Person person in travelParticipants)
        {
            SplitParticipants.Add(
                new SplitParticipantViewModel(
                    Messenger,
                    person,
                    percentageData.PersonPortionDict.GetValueOrDefault(person.Id, 0)));
        }
        
        CanCustomSplit = true;
    }
    
    private void LoadEvenSplit(SplitEvenly evenlyData, IReadOnlyCollection<Person> travelParticipants)
    {
        if (evenlyData.SplitParticipants.Count == 0)
        {
            evenlyData.SplitParticipants.AddRange(travelParticipants.Select(p => p.Id));
        }
        
        foreach (Person person in travelParticipants)
        {
            bool isSplitParticipant = evenlyData.SplitParticipants.Any(p => p.Equals(person.Id));
            
            SplitParticipants.Add(
                new SplitParticipantViewModel(
                    Messenger,
                    person,
                    isSplitParticipant ?
                        1m / evenlyData.SplitParticipants.Count :
                        0)
                {
                    Enable = isSplitParticipant
                });
        }
        
        CanCustomSplit = false;
    }
    
    private void CurrencyChanged(object recipient, PropertyChangedMessage<CurrencyModel> message)
    {
        Currency = message.NewValue;
    }
    
    private void ParticipantEnableChanged(object recipient, PropertyChangedMessage<bool> message)
    {
        if (!isLoaded || CanCustomSplit)
            return;
        RedistributeValue();
    }
    
    private void ValueChanged(object recipient, PropertyChangedMessage<decimal> message)
    {
        if (!isLoaded)
            return;
        Validate();
    }
    
    private void TotalValueChanged(object recipient, PaymentTotalValueChangedMessage message)
    {
        if (!isLoaded)
            return;
        TotalPrice = message.newValue;
    }
    
    private void RedistributeValue()
    {
        int count = SplitParticipants.Count(p => p.Enable);
        decimal targetValue = 1m / count;
        foreach (SplitParticipantViewModel vm in SplitParticipants)
        {
            vm.Value = vm.Enable ? targetValue : 0;
        }
    }
    
    private void Validate()
    {
        HasError = CanCustomSplit && SplitParticipants.Sum(vm => vm.Value) != 1;
    }
    
    public override ISplitData Save()
    {
        return CanCustomSplit ? SavePercentageSplit() : SaveEvenSplit();
    }
    
    private ISplitData SavePercentageSplit()
    {
        SplitByPercentage splitData = new();
        foreach (SplitParticipantViewModel sp in SplitParticipants)
        {
            if (sp.Value > 0)
                splitData.PersonPortionDict[sp.Person.Id] = sp.Value;
        }
        
        decimal sum = splitData.PersonPortionDict.Sum(kvp => kvp.Value);
        if (sum != 1)
        {
            decimal diff = 1 - sum;
            string personKey = splitData.PersonPortionDict.MaxBy(kvp => kvp.Value).Key;
            splitData.PersonPortionDict[personKey] += diff;
        }
        
        return splitData;
    }
    
    private ISplitData SaveEvenSplit()
    {
        SplitEvenly splitEvenly = new();
        splitEvenly.SplitParticipants.AddRange(SplitParticipants.Where(vm => vm.Enable).Select(vm => vm.Person.Id));
        return splitEvenly;
    }
}