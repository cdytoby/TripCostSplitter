using CommunityToolkit.Mvvm.Messaging;
using TripCostSplitter.AppBase.ViewModels.SplitViewModels;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.AppBase.Services;

public class SplitDataViewModelService
{
    public static List<string> GetAvaliableSplitMethods()
    {
        return [SplitByExactAmount.Key, SplitByPercentage.Key, SplitByItemOwnership.Key, SplitEvenly.Key];
    }
    
    public (string splitMethod, SplitDataViewModelBase? viewModel) LoadSplitDataViewModel(
        PaymentData paymentData,
        IReadOnlyList<Person> travelParticipants,
        CurrencyModel currency,
        IMessenger? messenger = null)
    {
        string splitMethod = string.Empty;
        SplitDataViewModelBase? viewModel = null;
        
        switch (paymentData.SplitData)
        {
            case SplitByExactAmount:
                splitMethod = SplitByExactAmount.Key;
                viewModel = new SplitByExactAmountViewModel(messenger);
                break;
            case SplitByPercentage:
                splitMethod = SplitByPercentage.Key;
                viewModel = new SplitByPercentageViewModel(messenger);
                break;
            case SplitByItemOwnership:
                splitMethod = SplitByItemOwnership.Key;
                viewModel = new SplitByItemOwnershipViewModel(messenger);
                break;
            case SplitEvenly:
                splitMethod = SplitEvenly.Key;
                viewModel = new SplitByPercentageViewModel(messenger);
                break;
        }
        
        viewModel?.Load(paymentData, travelParticipants, currency);
        
        return (splitMethod, viewModel);
    }
    
    public SplitDataViewModelBase? LoadSplitDataViewModel(
        string? splitMethod,
        PaymentData paymentData,
        IReadOnlyList<Person> travelParticipants,
        CurrencyModel currency,
        IMessenger? messenger = null)
    {
        decimal totalPrice = paymentData.PayerInfos.Sum(pi => pi.Amount);
        SplitDataViewModelBase? SplitDataViewModel;
        
        switch (splitMethod)
        {
            case SplitByExactAmount.Key:
                SplitDataViewModel = new SplitByExactAmountViewModel(messenger);
                SplitByExactAmount exactAmountData = new();
                foreach (Person traveller in travelParticipants)
                {
                    exactAmountData.PersonIdAmountDict.Add(traveller.Id, totalPrice / travelParticipants.Count);
                }
                
                paymentData.SplitData = exactAmountData;
                SplitDataViewModel.Load(paymentData, travelParticipants, currency);
                break;
            case SplitByPercentage.Key:
                SplitDataViewModel = new SplitByPercentageViewModel(messenger);
                SplitByPercentage percentageData = new();
                foreach (Person traveller in travelParticipants)
                {
                    percentageData.PersonPortionDict.Add(traveller.Id, 1m / travelParticipants.Count);
                }
                
                paymentData.SplitData = percentageData;
                SplitDataViewModel.Load(paymentData, travelParticipants, currency);
                break;
            case SplitByItemOwnership.Key:
                SplitDataViewModel = new SplitByItemOwnershipViewModel(messenger);
                SplitByItemOwnership ownershipData = new();
                ownershipData.OwnershipGroups.Add(travelParticipants.First().Id,
                    [..paymentData.PurchaseItems.Select(i => i.ItemName)]);
                
                paymentData.SplitData = ownershipData;
                SplitDataViewModel.Load(paymentData, travelParticipants, currency);
                break;
            case SplitEvenly.Key:
                SplitDataViewModel = new SplitByPercentageViewModel(messenger);
                SplitEvenly evenData = new();
                foreach (Person traveller in travelParticipants)
                {
                    evenData.SplitParticipants.Add(traveller.Id);
                }
                
                paymentData.SplitData = evenData;
                SplitDataViewModel.Load(paymentData, travelParticipants, currency);
                break;
            default:
                SplitDataViewModel = null;
                break;
        }
        
        return SplitDataViewModel;
    }
}