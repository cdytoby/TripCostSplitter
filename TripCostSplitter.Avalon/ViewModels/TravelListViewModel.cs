using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.Avalon.Services;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Avalon.ViewModels;

public partial class TravelListViewModel: ObservableObject
{
    private readonly MainViewModel main;
    private readonly AccessManager accessManager;
    
    public ObservableCollection<Travel> Travels => main.Travels;
    
    public TravelListViewModel(MainViewModel _main, AccessManager _accessManager)
    {
        main = _main;
        accessManager = _accessManager;
    }
    
    [RelayCommand]
    public async Task AddTravel()
    {
        Travel travel = new()
        {
            TravelId = accessManager.GetNextId(),
            Name = "New Trip",
            CalculateCurrency = "USD",
            Transactions = []
        };
        main.Travels.Add(travel);
        await main.SaveDataCommand.ExecuteAsync(null);
        main.CurrentViewModel = main.CreateViewModel<TravelDetailViewModel>(travel);
    }
    
    [RelayCommand]
    public void EditTravel(Travel travel)
    {
        main.CurrentViewModel = main.CreateViewModel<TravelDetailViewModel>(travel);
    }
    
    [RelayCommand]
    public async Task DeleteTravel(Travel travel)
    {
        main.Travels.Remove(travel);
        await main.SaveDataCommand.ExecuteAsync(null);
    }
}