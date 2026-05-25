using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.AppBase.ViewModels;

public partial class TravelListViewModel: ObservableObject
{
    private readonly SessionService sessionService;
    private readonly IAppDispatcherService dispatcher;
    private readonly INavigationService navigationService;
    
    public ObservableCollection<Travel> Travels { get; } = new();
    
    public TravelListViewModel(
        IAppDispatcherService _dispatcher,
        INavigationService _navigationService,
        SessionService _sessionService)
    {
        dispatcher = _dispatcher;
        navigationService = _navigationService;
        sessionService = _sessionService;
        
        Load();
    }
    
    private void Load()
    {
        IEnumerable<Travel> loadedTravels = sessionService.GetAllTravels();
        
        foreach (Travel travel in loadedTravels)
        {
            Travels.Add(travel);
        }
    }
    
    [RelayCommand]
    private async Task AddTravel()
    {
        Travel newTravel = new()
        {
            TravelId = AccessManager.GetNewId(),
            Name = "New Trip",
            CalculateCurrency = "USD"
        };
        Travels.Add(newTravel);
        sessionService.CurrentTravel = newTravel;
        await sessionService.SaveTravel();
        await navigationService.PushAsync(ViewDefinition.TravelDetailView);
    }
    
    [RelayCommand]
    private async Task EditTravel(Travel editTravel)
    {
        sessionService.CurrentTravel = editTravel;
        await navigationService.PushAsync(ViewDefinition.TravelDetailView);
    }
    
    [RelayCommand]
    private async Task DeleteTravel(Travel travel)
    {
        Travels.Remove(travel);
        await sessionService.DeleteTravel(travel);
    }

    [RelayCommand]
    private async Task NavigateToSettings()
    {
        await navigationService.PushAsync(ViewDefinition.SettingsView);
    }
}