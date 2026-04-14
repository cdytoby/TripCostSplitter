using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.Avalon.Services;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.Avalon.ViewModels;

public partial class TransactionDetailViewModel : ObservableObject
{
    private readonly MainViewModel _main;
    private readonly TravelDetailViewModel _travelDetail;
    private readonly IEnumerable<ISplitCalculator> _splitCalculators;
    private readonly INavigationService _navigationService;
    public Transaction Transaction { get; }
    public PaymentData? PaymentData => Transaction.TransactionData as PaymentData;

    [ObservableProperty]
    public partial ObservableCollection<PayerViewModel> Payers { get; set; }

    [ObservableProperty]
    public partial bool IsSplitEvenly { get; set; }

    [ObservableProperty]
    public partial bool IsSplitExact { get; set; }

    [ObservableProperty]
    public partial bool IsSplitPercentage { get; set; }

    [ObservableProperty]
    public partial bool IsSplitByItemOwnership { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<SplitParticipantViewModel> SplitParticipants { get; set; }
    
    [ObservableProperty]
    public partial ObservableCollection<PurchaseItemViewModel> Items { get; set; }

    public TransactionDetailViewModel(MainViewModel main, TravelDetailViewModel travelDetail, Transaction transaction, IEnumerable<ISplitCalculator> splitCalculators, INavigationService navigationService)
    {
        _main = main;
        _travelDetail = travelDetail;
        _splitCalculators = splitCalculators;
        _navigationService = navigationService;
        Transaction = transaction;
        Payers = [];
        SplitParticipants = [];
        Items = [];
        IsSplitEvenly = true;
        IsSplitExact = false;
        IsSplitPercentage = false;
        IsSplitByItemOwnership = false;

        if (PaymentData != null)
        {
            foreach (Person person in _travelDetail.Participants)
            {
                PayerInfo? payerInfo = PaymentData.PayerInfos.FirstOrDefault(p => p.PayerId == person.Id);
                Payers.Add(new PayerViewModel(person, payerInfo?.Amount ?? 0));
                
                SplitParticipants.Add(new SplitParticipantViewModel(person));
            }

            foreach (PurchaseItem item in PaymentData.PurchaseItems)
            {
                Items.Add(new PurchaseItemViewModel(item, _travelDetail.Participants));
            }

            // Initialize split method from existing data
            if (PaymentData.SplitData is SplitByExactAmount exact)
            {
                IsSplitEvenly = false;
                IsSplitExact = true;
                foreach (SplitParticipantViewModel sp in SplitParticipants)
                {
                    if (exact.PersonIdAmountDict.TryGetValue(sp.Person.Id, out decimal amount))
                        sp.Value = amount;
                }
            }
            else if (PaymentData.SplitData is SplitByPercentage percentage)
            {
                IsSplitEvenly = false;
                IsSplitPercentage = true;
                foreach (SplitParticipantViewModel sp in SplitParticipants)
                {
                    if (percentage.PersonPercentageDict.TryGetValue(sp.Person.Id, out decimal p))
                        sp.Value = p;
                }
            }
            else if (PaymentData.SplitData is SplitByItemOwnership ownership)
            {
                IsSplitEvenly = false;
                IsSplitByItemOwnership = true;
                foreach (KeyValuePair<int, List<string>> kvp in ownership.OwnershipGroups)
                {
                    int personId = kvp.Key;
                    foreach (string itemName in kvp.Value)
                    {
                        PurchaseItemViewModel? itemVm = Items.FirstOrDefault(i => i.Item.Item == itemName);
                        if (itemVm != null)
                        {
                            ItemParticipantViewModel? participant = itemVm.Participants.FirstOrDefault(p => p.Person.Id == personId);
                            if (participant != null) participant.IsSelected = true;
                        }
                    }
                }
            }
            else
            {
                IsSplitEvenly = true;
            }
        }
    }

    [RelayCommand]
    public async Task Save()
    {
        if (PaymentData != null)
        {
            PaymentData.PayerInfos.Clear();
            foreach (PayerViewModel pvm in Payers.Where(p => p.Amount > 0))
            {
                PaymentData.PayerInfos.Add(new PayerInfo(pvm.Person.Id, pvm.Amount));
            }

            PaymentData.PurchaseItems.Clear();
            foreach (PurchaseItemViewModel itemVm in Items)
            {
                PaymentData.PurchaseItems.Add(itemVm.Item);
            }

            if (IsSplitExact)
            {
                SplitByExactAmount exact = new();
                foreach (SplitParticipantViewModel sp in SplitParticipants)
                {
                    exact.PersonIdAmountDict[sp.Person.Id] = sp.Value;
                }
                PaymentData.SplitData = exact;
            }
            else if (IsSplitPercentage)
            {
                SplitByPercentage percentage = new();
                foreach (SplitParticipantViewModel sp in SplitParticipants)
                {
                    percentage.PersonPercentageDict[sp.Person.Id] = sp.Value;
                }
                PaymentData.SplitData = percentage;
            }
            else if (IsSplitByItemOwnership)
            {
                SplitByItemOwnership ownership = new();
                foreach (PurchaseItemViewModel itemVm in Items)
                {
                    foreach (ItemParticipantViewModel part in itemVm.Participants.Where(p => p.IsSelected))
                    {
                        if (!ownership.OwnershipGroups.ContainsKey(part.Person.Id))
                            ownership.OwnershipGroups[part.Person.Id] = [];
                        
                        ownership.OwnershipGroups[part.Person.Id].Add(itemVm.Item.Item);
                    }
                }
                PaymentData.SplitData = ownership;
            }
            else
            {
                PaymentData.SplitData = new SplitEvenly();
            }

            ISplitCalculator? calculator = _splitCalculators.FirstOrDefault(c => c.CanHandle(PaymentData.SplitData));

            if (calculator != null)
            {
                IList<RecipientInfo> debits = calculator.CalculateDebit(PaymentData);
                Transaction.RecipientInfos = new(debits);
            }
        }
        
        _travelDetail.UpdateDebts();
        await _navigationService.PopAsync();
    }

    [RelayCommand]
    public void AddItem()
    {
        PurchaseItem newItem = new("New Item", 0);
        Items.Add(new PurchaseItemViewModel(newItem, _travelDetail.Participants));
    }

    [RelayCommand]
    public void RemoveItem(PurchaseItemViewModel item)
    {
        Items.Remove(item);
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await _navigationService.PopAsync();
    }
}