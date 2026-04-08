using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Avalon.ViewModels;

public partial class DebtResultViewModel : ObservableObject
{
    private readonly MainViewModel _main;
    private readonly TravelDetailViewModel _travelDetail;

    [ObservableProperty]
    public partial ObservableCollection<DebtDisplayItem> Debts { get; set; }

    public DebtResultViewModel(MainViewModel main, TravelDetailViewModel travelDetail, List<DebtItem> debts)
    {
        _main = main;
        _travelDetail = travelDetail;
        Debts = new();

        foreach (DebtItem debt in debts)
        {
            string debtorName = _travelDetail.Participants.FirstOrDefault(p => p.Id == debt.DebtorId)?.Name ?? $"ID {debt.DebtorId}";
            string creditorName = _travelDetail.Participants.FirstOrDefault(p => p.Id == debt.CreditorId)?.Name ?? $"ID {debt.CreditorId}";
            Debts.Add(new DebtDisplayItem(debtorName, creditorName, debt.Amount));
        }
    }

    [RelayCommand]
    public void Back()
    {
        _main.CurrentViewModel = _travelDetail;
    }
}