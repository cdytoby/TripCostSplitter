using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.Services;

public interface IDataService
{
    Task SaveTravelAsync(Travel travel);
    
    Task SaveAllTravelsAsync(IEnumerable<Travel> travels);
    
    Task<IEnumerable<Travel>> LoadAllTravelsAsync();
    
    Task DeleteTravelAsync(Travel travel);
}