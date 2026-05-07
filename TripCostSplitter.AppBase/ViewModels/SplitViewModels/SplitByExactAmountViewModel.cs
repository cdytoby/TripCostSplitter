using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using TripCostSplitter.AppBase.Messages;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.AppBase.ViewModels.SplitViewModels;

public partial class SplitByExactAmountViewModel: SplitDataViewModelBase
{
    [ObservableProperty]
    public partial ObservableCollection<SplitParticipantViewModel> SplitParticipants { get; set; } = [];
    
    [ObservableProperty]
    public partial CurrencyModel? Currency { get; set; }
    
    [ObservableProperty]
    public partial bool HasError { get; set; } = false;
    
    private PaymentData? loadedData { get; set; }
    private bool isLoaded => loadedData != null;
    
    public SplitByExactAmountViewModel(IMessenger? messenger = null): base(messenger)
    {
        Messenger.Register<PropertyChangedMessage<CurrencyModel>>(this, CurrencyChanged);
        Messenger.Register<PropertyChangedMessage<decimal>>(this, ValueChanged);
        Messenger.Register<PaymentTotalValueChangedMessage>(this, TotalValueChanged);
    }
    
    public override void Load(
        PaymentData paymentData, IReadOnlyCollection<Person> travelParticipants, CurrencyModel useCurrency)
    {
        if (paymentData.SplitData is SplitByExactAmount exactAmountData)
        {
            foreach (Person participant in travelParticipants)
            {
                SplitParticipants.Add(
                    new SplitParticipantViewModel(
                        Messenger,
                        participant,
                        exactAmountData.PersonIdAmountDict.GetValueOrDefault(participant.Id)));
            }
        }
        
        Currency = useCurrency;
        loadedData = paymentData;
    }
    
    private void CurrencyChanged(object recipient, PropertyChangedMessage<CurrencyModel> message)
    {
        Currency = message.NewValue;
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
        Validate();
    }
    
    private void Validate()
    {
        HasError = SplitParticipants.Sum(vm => vm.Value) != loadedData!.PayerInfos.Sum(pi => pi.Amount);
    }
    
    public override ISplitData Save()
    {
        SplitByExactAmount splitData = new();
        if (!isLoaded)
            return splitData;
        foreach (SplitParticipantViewModel sp in SplitParticipants)
        {
            if (sp.Value > 0)
            {
                splitData.PersonIdAmountDict[sp.Person.Id] = sp.Value;
            }
        }
        
        decimal sum = splitData.PersonIdAmountDict.Sum(kvp => kvp.Value);
        decimal targetSum = loadedData!.PayerInfos.Sum(pi => pi.Amount);
        
        if (sum == targetSum)
        {
            return splitData;
        }
        
        decimal diff = targetSum - sum;
        string personKey = splitData.PersonIdAmountDict.MaxBy(kvp => kvp.Value).Key;
        splitData.PersonIdAmountDict[personKey] += diff;
        
        return splitData;
    }
}