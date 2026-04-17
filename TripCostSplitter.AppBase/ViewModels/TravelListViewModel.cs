using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.AppBase.ViewModels;

public partial class TravelListViewModel: ObservableObject
{
    private readonly AccessManager accessManager;
    private readonly SessionService sessionService;
    private readonly IAppDispatcherService dispatcher;
    private readonly INavigationService navigationService;
    private readonly IDataService dataService;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddTravelCommand))]
    private partial bool Loaded { get; set; } = false;
    
    public ObservableCollection<Travel> Travels { get; } = new();
    
    public TravelListViewModel(
        AccessManager _accessManager,
        IAppDispatcherService _dispatcher,
        INavigationService _navigationService,
        IDataService _dataService,
        SessionService _sessionService)
    {
        accessManager = _accessManager;
        dispatcher = _dispatcher;
        navigationService = _navigationService;
        dataService = _dataService;
        sessionService = _sessionService;
        Task.Run(Load);
    }
    
    private async Task Load()
    {
        IEnumerable<Travel> loadedTravels = await dataService.LoadAllTravelsAsync();
        
        dispatcher.Invoke(() =>
        {
            foreach (Travel travel in loadedTravels)
            {
                Travels.Add(travel);
            }
            
            Loaded = true;
        });
    }
    
    [RelayCommand(CanExecute = nameof(Loaded))]
    private async Task AddTravel()
    {
        Travel newTravel = new()
        {
            TravelId = accessManager.GetNextId(),
            Name = "New Trip",
            CalculateCurrency = "USD"
        };
        Travels.Add(newTravel);
        await dataService.SaveTravelAsync(newTravel);
        sessionService.CurrentTravel = newTravel;
        await navigationService.PushAsync(ViewDefinition.TravelDetailView);
    }
    
    [RelayCommand(CanExecute = nameof(Loaded))]
    private async Task EditTravel(Travel editTravel)
    {
        sessionService.CurrentTravel = editTravel;
        await navigationService.PushAsync(ViewDefinition.TravelDetailView);
    }
    
    [RelayCommand(CanExecute = nameof(Loaded))]
    private async Task DeleteTravel(Travel travel)
    {
        Travels.Remove(travel);
        await dataService.DeleteTravelAsync(travel);
    }
}