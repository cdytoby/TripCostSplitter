using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.Core;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.AppBase.ViewModels;

public partial class TravelDetailViewModel: ObservableObject
{
    private readonly AccessManager accessManager;
    private readonly SessionService sessionService;
    private readonly INavigationService navigationService;
    
    public Travel Travel { get; }
    public CurrencyModel[] AllCurrencies { get; }
    
    [ObservableProperty]
    public partial ObservableCollection<Person> Participants { get; set; }
    
    [ObservableProperty]
    public partial ObservableCollection<DebtDisplayItem> Debts { get; set; }
    
    public TravelDetailViewModel(
        AccessManager _accessManager,
        INavigationService _navigationService,
        SessionService _sessionService,
        CurrencyService _currencyService)
    {
        accessManager = _accessManager;
        navigationService = _navigationService;
        sessionService = _sessionService;
        
        AllCurrencies = _currencyService.GetAllCurrencyInfos();
        
        //todo exception or load state with nullable
        Travel = sessionService.CurrentTravel!;
        
        Participants = [];
        Debts = [];
        
        // In a real app, we'd load participants from somewhere.
        // For now, let's add some default ones if empty.
        if (Participants.Count == 0)
        {
            Participants.Add(new Person(1, "Alice"));
            Participants.Add(new Person(2, "Bob"));
        }
        
        UpdateDebts();
    }
    
    [RelayCommand]
    private void AddAdditionalCurrency(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return;
        if (Travel.CalculateCurrency == code)
            return;
        if (Travel.AdditionalCurrencies.Contains(code))
            return;
        
        Travel.AdditionalCurrencies.Add(code);
    }
    
    [RelayCommand]
    private void DeleteAdditionalCurrency(string code)
    {
        Travel.AdditionalCurrencies.Remove(code);
    }
    
    [RelayCommand]
    public void UpdateDebts()
    {
        DebtCalculator calculator = new();
        List<DebtItem> debtsResult = calculator.CalculateDebts(Travel).ToList();
        
        Debts.Clear();
        foreach (DebtItem debt in debtsResult)
        {
            string debtorName = Participants.FirstOrDefault(p => p.Id == debt.DebtorId)?.Name ?? $"ID {debt.DebtorId}";
            string creditorName = Participants.FirstOrDefault(p => p.Id == debt.CreditorId)?.Name ??
                $"ID {debt.CreditorId}";
            Debts.Add(new DebtDisplayItem(debtorName, creditorName, debt.Amount));
        }
    }
    
    [RelayCommand]
    public void AddPerson(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        int newId = Participants.Count > 0 ? Participants.Max(p => p.Id) + 1 : 1;
        Participants.Add(new Person(newId, name));
        UpdateDebts();
    }
    
    //todo delete person when a person doesn't involve any transactions
    
    [RelayCommand]
    public async Task AddTransaction()
    {
        PaymentData transactionData = new()
        {
            Date = DateTime.Now,
            Currency = Travel.CalculateCurrency,
            PayerInfos = [],
            ParticipantIds = new(Participants.Select(p => p.Id)),
            PurchaseItems = []
        };
        Transaction transaction = new()
        {
            TransactionId = accessManager.GetNextId(),
            TransactionData = transactionData,
            RecipientInfos = []
        };
        Travel.Transactions.Add(transaction);
        await navigationService.PushAsync(ViewDefinition.TransactionDetailView);
    }
    
    [RelayCommand]
    public async Task EditTransaction(Transaction transaction)
    {
        await navigationService.PushAsync(ViewDefinition.TransactionDetailView);
    }
    
    [RelayCommand]
    public void DeleteTransaction(Transaction transaction)
    {
        Travel.Transactions.Remove(transaction);
        UpdateDebts();
    }
    
    [RelayCommand]
    public async Task ViewDebts()
    {
        DebtCalculator calculator = new();
        List<DebtItem> debts = calculator.CalculateDebts(Travel).ToList();
        // await navigationService.PushAsync<DebtResultViewModel>(this, debts);
    }
    
    [RelayCommand]
    public async Task Back()
    {
        // await main.SaveDataCommand.ExecuteAsync(null);
        await navigationService.PopAsync();
    }
}