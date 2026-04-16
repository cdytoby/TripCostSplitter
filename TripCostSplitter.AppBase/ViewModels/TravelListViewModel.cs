using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.AppBase.ViewModels;

public partial class TravelListViewModel: ObservableObject
{
    private readonly AccessManager accessManager;
    private readonly INavigationService navigationService;
    
    public ObservableCollection<Travel> Travels = new();
    
    public TravelListViewModel(AccessManager _accessManager, INavigationService _navigationService)
    {
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
        // main.Travels.Add(travel);
        // await main.SaveDataCommand.ExecuteAsync(null);
        await navigationService.PushAsync(ViewDefinition.TravelDetailView);
    }
    
    [RelayCommand]
    public async Task EditTravel(Travel travel)
    {
        await navigationService.PushAsync(ViewDefinition.TravelDetailView);
    }
    
    [RelayCommand]
    public async Task DeleteTravel(Travel travel)
    {
        // main.Travels.Remove(travel);
        // await main.SaveDataCommand.ExecuteAsync(null);
    }
}