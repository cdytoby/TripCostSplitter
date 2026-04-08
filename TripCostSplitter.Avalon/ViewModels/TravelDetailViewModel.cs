using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.Core;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Avalon.ViewModels;

public partial class TravelDetailViewModel : ObservableObject
{
    private readonly MainViewModel _main;
    public Travel Travel { get; }

    [ObservableProperty]
    public partial ObservableCollection<Person> Participants { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<DebtDisplayItem> Debts { get; set; }

    public TravelDetailViewModel(MainViewModel main, Travel travel)
    {
        _main = main;
        Travel = travel;
        Participants = new();
        Debts = new();
        
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
    public void UpdateDebts()
    {
        DebtCalculator calculator = new DebtCalculator();
        List<DebtItem> debtsResult = calculator.CalculateDebts(Travel).ToList();
        
        Debts.Clear();
        foreach (DebtItem debt in debtsResult)
        {
            string debtorName = Participants.FirstOrDefault(p => p.Id == debt.DebtorId)?.Name ?? $"ID {debt.DebtorId}";
            string creditorName = Participants.FirstOrDefault(p => p.Id == debt.CreditorId)?.Name ?? $"ID {debt.CreditorId}";
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

    [RelayCommand]
    public void AddTransaction()
    {
        PaymentData transactionData = new PaymentData
        {
            Date = DateTime.Now,
            Currency = Travel.CalculateCurrency,
            PayerInfos = new(),
            ParticipantIds = new(Participants.Select(p => p.Id)),
            PurchaseItems = new()
        };
        Transaction transaction = new Transaction
        {
            TransactionData = transactionData,
            RecipientInfos = new()
        };
        Travel.Transactions.Add(transaction);
        _main.CurrentViewModel = _main.CreateViewModel<TransactionDetailViewModel>(this, transaction);
    }

    [RelayCommand]
    public void EditTransaction(Transaction transaction)
    {
        _main.CurrentViewModel = _main.CreateViewModel<TransactionDetailViewModel>(this, transaction);
    }

    [RelayCommand]
    public void DeleteTransaction(Transaction transaction)
    {
        Travel.Transactions.Remove(transaction);
        UpdateDebts();
    }

    [RelayCommand]
    public void ViewDebts()
    {
        DebtCalculator calculator = new DebtCalculator();
        List<DebtItem> debts = calculator.CalculateDebts(Travel).ToList();
        _main.CurrentViewModel = _main.CreateViewModel<DebtResultViewModel>(this, debts);
    }

    [RelayCommand]
    public void Back()
    {
        _main.GoBack();
    }
}