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
        IReadOnlyList<Person> TravelParticipants, PaymentData PaymentData)
    {
        string splitMethod = string.Empty;
        SplitDataViewModelBase? viewModel = null;
        
        switch (PaymentData.SplitData)
        {
            case SplitByExactAmount splitDataExactAmount:
                splitMethod = SplitByExactAmount.Key;
                viewModel = new SplitByExactAmountViewModel();
                viewModel.Load(splitDataExactAmount, TravelParticipants, PaymentData);
                break;
            case SplitByPercentage splitDataPercentage:
                splitMethod = SplitByPercentage.Key;
                viewModel = new SplitByPercentageViewModel();
                viewModel.Load(splitDataPercentage, TravelParticipants, PaymentData);
                break;
            case SplitByItemOwnership ownership:
                splitMethod = SplitByItemOwnership.Key;
                viewModel = new SplitByItemOwnershipViewModel();
                viewModel.Load(ownership, TravelParticipants, PaymentData);
                break;
            case SplitEvenly evenSplitData:
                splitMethod = SplitEvenly.Key;
                viewModel = new SplitByPercentageViewModel();
                viewModel.Load(evenSplitData, TravelParticipants, PaymentData);
                break;
        }
        
        return (splitMethod, viewModel);
    }
    
    public SplitDataViewModelBase? LoadSplitDataViewModel(
        string? splitMethod, IReadOnlyList<Person> TravelParticipants, PaymentData PaymentData)
    {
        decimal totalPrice = PaymentData.PayerInfos.Sum(pi => pi.Amount);
        SplitDataViewModelBase? SplitDataViewModel;
        
        switch (splitMethod)
        {
            case SplitByExactAmount.Key:
                SplitDataViewModel = new SplitByExactAmountViewModel();
                SplitByExactAmount exactAmountData = new();
                foreach (Person traveller in TravelParticipants)
                {
                    exactAmountData.PersonIdAmountDict.Add(traveller.Id, totalPrice / TravelParticipants.Count);
                }
                
                SplitDataViewModel.Load(exactAmountData, TravelParticipants, PaymentData);
                break;
            case SplitByPercentage.Key:
                SplitDataViewModel = new SplitByPercentageViewModel();
                SplitByPercentage percentageData = new();
                foreach (Person traveller in TravelParticipants)
                {
                    percentageData.PersonPercentageDict.Add(traveller.Id, 100m / TravelParticipants.Count);
                }
                
                SplitDataViewModel.Load(percentageData, TravelParticipants, PaymentData);
                break;
            case SplitByItemOwnership.Key:
                SplitDataViewModel = new SplitByItemOwnershipViewModel();
                SplitByItemOwnership ownershipData = new();
                ownershipData.OwnershipGroups.Add(TravelParticipants.First().Id,
                    [..PaymentData.PurchaseItems.Select(i => i.Item)]);
                
                SplitDataViewModel.Load(ownershipData, TravelParticipants, PaymentData);
                break;
            case SplitEvenly.Key:
                SplitDataViewModel = new SplitByPercentageViewModel();
                SplitEvenly evenData = new();
                foreach (Person traveller in TravelParticipants)
                {
                    evenData.SplitParticipants.Add(traveller.Id);
                }
                
                SplitDataViewModel.Load(evenData, TravelParticipants, PaymentData);
                break;
            default:
                SplitDataViewModel = null;
                break;
        }
        
        return SplitDataViewModel;
    }
}