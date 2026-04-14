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
    private readonly INavigationService navigationService;
    
    public ObservableCollection<Travel> Travels => main.Travels;
    
    public TravelListViewModel(MainViewModel _main, AccessManager _accessManager, INavigationService _navigationService)
    {
        main = _main;
        accessManager = _accessManager;
        navigationService = _navigationService;
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
        await navigationService.PushAsync<TravelDetailViewModel>(travel);
    }
    
    [RelayCommand]
    public async Task EditTravel(Travel travel)
    {
        await navigationService.PushAsync<TravelDetailViewModel>(travel);
    }
    
    [RelayCommand]
    public async Task DeleteTravel(Travel travel)
    {
        main.Travels.Remove(travel);
        await main.SaveDataCommand.ExecuteAsync(null);
    }
}