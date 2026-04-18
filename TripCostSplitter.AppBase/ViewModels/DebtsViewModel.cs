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
    private readonly SessionService sessionService;
    
    public Travel Travel { get; }
    
    [ObservableProperty]
    public partial ObservableCollection<DebtDisplayItem> Debts { get; set; }
    
    public DebtsViewModel(
        SessionService _sessionService)
    {
        sessionService = _sessionService;
        
        //todo exception or load state with nullable
        Travel = sessionService.CurrentTravel!;
        
        Debts = [];
        
        UpdateDebts();
    }
    
    [RelayCommand]
    public void UpdateDebts()
    {
        DebtCalculator calculator = new();
        List<DebtItem> debtsResult = calculator.CalculateDebts(Travel).ToList();
        
        Debts.Clear();
        foreach (DebtItem debt in debtsResult)
        {
            string debtorName = Travel.Participants.FirstOrDefault(p => p.Id == debt.DebtorId)?.Name ??
                $"ID {debt.DebtorId}";
            string creditorName = Travel.Participants.FirstOrDefault(p => p.Id == debt.CreditorId)?.Name ??
                $"ID {debt.CreditorId}";
            Debts.Add(new DebtDisplayItem(debtorName, creditorName, debt.Amount));
        }
    }
}