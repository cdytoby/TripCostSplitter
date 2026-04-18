using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.AppBase.ViewModels;

public partial class TransactionDetailViewModel: ObservableObject
{
    public List<CurrencyModel> AvailableCurrencies { get; }
    
    public PaymentData? PaymentData => transaction.TransactionData as PaymentData;
    
    [ObservableProperty]
    public partial ObservableCollection<PayerViewModel> Payers { get; set; }
    
    [ObservableProperty]
    public partial ObservableCollection<SplitParticipantViewModel> SplitParticipants { get; set; }
    
    [ObservableProperty]
    public partial ObservableCollection<PurchaseItemViewModel> Items { get; set; }
    
    private string? splitMethod;
    private readonly List<ISplitCalculator> splitCalculators;
    private readonly INavigationService navigationService;
    private Transaction transaction;
    private List<Person> participants;
    
    public TransactionDetailViewModel(
        SessionService _sessionService,
        IEnumerable<ISplitCalculator> _splitCalculators,
        INavigationService _navigationService,
        CurrencyService _currencyService)
    {
        splitCalculators = _splitCalculators.ToList();
        navigationService = _navigationService;
        
        //todo exception or load state with nullable
        transaction = _sessionService.CurrentTransaction!;
        participants = _sessionService.CurrentTravel!.Participants.ToList();
        AvailableCurrencies =
        [
            .._currencyService.GetCurrencyInfos(
                [_sessionService.CurrentTravel.CalculateCurrency, .._sessionService.CurrentTravel.AdditionalCurrencies])
        ];
        
        Payers = [];
        SplitParticipants = [];
        Items = [];
        
        if (PaymentData != null)
        {
            foreach (Person person in participants)
            {
                PayerInfo? payerInfo = PaymentData.PayerInfos.FirstOrDefault(p => p.PayerId == person.Id);
                Payers.Add(new PayerViewModel(person, payerInfo?.Amount ?? 0));
                
                SplitParticipants.Add(new SplitParticipantViewModel(person));
            }
            
            foreach (PurchaseItem item in PaymentData.PurchaseItems)
            {
                Items.Add(new PurchaseItemViewModel(item, participants));
            }
            
            // Initialize split method from existing data
            if (PaymentData.SplitData is SplitByExactAmount exact)
            {
                splitMethod = SplitByExactAmount.Key;
                foreach (SplitParticipantViewModel sp in SplitParticipants)
                {
                    if (exact.PersonIdAmountDict.TryGetValue(sp.Person.Id, out decimal amount))
                        sp.Value = amount;
                }
            }
            else if (PaymentData.SplitData is SplitByPercentage percentage)
            {
                splitMethod = SplitByPercentage.Key;
                foreach (SplitParticipantViewModel sp in SplitParticipants)
                {
                    if (percentage.PersonPercentageDict.TryGetValue(sp.Person.Id, out decimal p))
                        sp.Value = p;
                }
            }
            else if (PaymentData.SplitData is SplitByItemOwnership ownership)
            {
                splitMethod = SplitByItemOwnership.Key;
                foreach (KeyValuePair<int, List<string>> kvp in ownership.OwnershipGroups)
                {
                    int personId = kvp.Key;
                    foreach (string itemName in kvp.Value)
                    {
                        PurchaseItemViewModel? itemVm = Items.FirstOrDefault(i => i.Item.Item == itemName);
                        if (itemVm != null)
                        {
                            ItemParticipantViewModel? participant =
                                itemVm.Participants.FirstOrDefault(p => p.Person.Id == personId);
                            if (participant != null) participant.IsSelected = true;
                        }
                    }
                }
            }
            else
            {
                splitMethod = SplitEvenly.Key;
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
            
            if (splitMethod.Equals(SplitByExactAmount.Key))
            {
                SplitByExactAmount exact = new();
                foreach (SplitParticipantViewModel sp in SplitParticipants)
                {
                    exact.PersonIdAmountDict[sp.Person.Id] = sp.Value;
                }
                
                PaymentData.SplitData = exact;
            }
            else if (splitMethod.Equals(SplitByPercentage.Key))
            {
                SplitByPercentage percentage = new();
                foreach (SplitParticipantViewModel sp in SplitParticipants)
                {
                    percentage.PersonPercentageDict[sp.Person.Id] = sp.Value;
                }
                
                PaymentData.SplitData = percentage;
            }
            else if (splitMethod.Equals(SplitByItemOwnership.Key))
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
            
            ISplitCalculator? calculator = splitCalculators.FirstOrDefault(c => c.CanHandle(PaymentData.SplitData));
            
            if (calculator != null)
            {
                IList<RecipientInfo> debits = calculator.CalculateDebit(PaymentData);
                transaction.RecipientInfos = new(debits);
            }
        }
        
        //todo update depts after pop
        await navigationService.PopAsync();
    }
    
    [RelayCommand]
    public void AddItem()
    {
        PurchaseItem newItem = new("New Item", 0);
        Items.Add(new PurchaseItemViewModel(newItem, participants));
    }
    
    [RelayCommand]
    public void RemoveItem(PurchaseItemViewModel item)
    {
        Items.Remove(item);
    }
    
    [RelayCommand]
    public async Task Cancel()
    {
        await navigationService.PopAsync();
    }
}