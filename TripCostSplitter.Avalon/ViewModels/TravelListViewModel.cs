using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Avalon.ViewModels;

public partial class TravelListViewModel : ObservableObject
{
    private readonly MainViewModel _main;

    public ObservableCollection<Travel> Travels => _main.Travels;

    public TravelListViewModel(MainViewModel main)
    {
        _main = main;
    }

    [RelayCommand]
    public async Task AddTravel()
    {
        Travel travel = new()
        {
            Name = "New Trip",
            CalculateCurrency = "USD",
            Transactions = []
        };
        _main.Travels.Add(travel);
        await _main.SaveDataCommand.ExecuteAsync(null);
        _main.CurrentViewModel = _main.CreateViewModel<TravelDetailViewModel>(travel);
    }

    [RelayCommand]
    public void EditTravel(Travel travel)
    {
        _main.CurrentViewModel = _main.CreateViewModel<TravelDetailViewModel>(travel);
    }

    [RelayCommand]
    public async Task DeleteTravel(Travel travel)
    {
        _main.Travels.Remove(travel);
        await _main.SaveDataCommand.ExecuteAsync(null);
    }
}