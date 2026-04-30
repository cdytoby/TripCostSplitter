using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.Services;

public interface IDataService
{
    IEnumerable<Travel> Travels { get; }
    
    SettingsDataModel Settings { get; }
    
    Task Load();
    
    Travel? GetTravel(string travelId);
    
    Task SaveTravelAsync(Travel newTravel);
    
    Task SaveAllTravelsAsync();
    
    Task DeleteTravelAsync(string travelId);
    
    Task SaveSettingsAsync();
}