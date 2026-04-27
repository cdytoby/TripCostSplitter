using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.Core;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.AppBase.ViewModels;

public partial class DebtsViewModel: ObservableObject
{
    public IReadOnlyList<Person> TravelParticipants { get; }
    
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
        Debts.Clear();
        DebtItem[] debtsResult = debtCalculator.CalculateDebts(travel).ToArray();
        foreach (DebtItem debtItem in debtsResult)
        {
            Debts.Add(debtItem);
        }
    }
}