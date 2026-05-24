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
    
    private readonly SessionService sessionService;
    private readonly DebtCalculator debtCalculator;
    private Travel travel;
    
    [ObservableProperty]
    public partial ObservableCollection<DebtItem> Debts { get; private set; } = [];
    
    public DebtsViewModel(
        SessionService _sessionService,
        DebtCalculator _debtCalculator)
    {
        sessionService = _sessionService;
        debtCalculator = _debtCalculator;
        
        //todo exception or load state with nullable
        travel = sessionService.CurrentTravel!;
        TravelParticipants = travel.Participants;
        
        UpdateDebts();
    }
    
    [RelayCommand]
    public void UpdateDebts()
    {
        Currency = CurrencyService.GetCurrencyInfo(travel.CalculateCurrency);
        Debts.Clear();
        DebtItem[] debtsResult = debtCalculator.CalculateDebts(travel).ToArray();
        foreach (DebtItem debtItem in debtsResult)
        {
            Debts.Add(debtItem);
        }
    }
}