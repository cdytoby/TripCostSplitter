using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.Core;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.AppBase.ViewModels;

public partial class DebtsViewModel: ObservableObject
{
    public IReadOnlyList<Person> TravelParticipants { get; }
    public CurrencyModel? Currency { get; private set; }
    
    public IRelayCommand UpdateDebtsCommand { get; }
    
    private readonly SessionService sessionService;
    private readonly DebtCalculator debtCalculator;
    private readonly CurrencyService currencyService;
    private Travel travel;
    
    [ObservableProperty]
    public partial ObservableCollection<DebtItem> Debts { get; private set; } = [];
    
    [ObservableProperty]
    public partial decimal TotalExpense { get; private set; }
    
    [ObservableProperty]
    public partial int TransactionCount { get; private set; }
    
    public DebtsViewModel(
        SessionService _sessionService,
        DebtCalculator _debtCalculator,
        CurrencyService _currencyService)
    {
        sessionService = _sessionService;
        debtCalculator = _debtCalculator;
        currencyService = _currencyService;
        
        UpdateDebtsCommand = new RelayCommand(UpdateDebts);
        
        //todo exception or load state with nullable
        travel = sessionService.CurrentTravel!;
        TravelParticipants = travel.Participants;
        
        UpdateDebts();
    }
    
    private void UpdateDebts()
    {
        Currency = CurrencyService.GetCurrencyInfo(travel.CalculateCurrency);
        Debts.Clear();
        DebtItem[] debtsResult = debtCalculator.CalculateDebts(travel).ToArray();
        foreach (DebtItem debtItem in debtsResult)
        {
            Debts.Add(debtItem);
        }
        
        TransactionCount = travel.Transactions.Count;
        TotalExpense = CalculateTotalExpense();
    }
    
    private decimal CalculateTotalExpense()
    {
        decimal total = 0m;
        foreach (Transaction transaction in travel.Transactions)
        {
            if (transaction.TransactionData is not PaymentData paymentData)
                continue;
            
            decimal rate = 1m;
            if (!travel.CalculateCurrency.Equals(transaction.Currency))
            {
                if (transaction.ExchangeRateOverride != null)
                {
                    rate = transaction.ExchangeRateOverride.Value;
                }
                else
                {
                    rate = currencyService.GetExchangeRate(transaction.Currency, travel.CalculateCurrency) ?? 0m;
                }
            }
            
            total += paymentData.PayerInfos.Sum(p => p.Amount) * rate;
        }
        return total;
    }
}