using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.Services;

public interface IDataService
{
    Task SaveAccessData(AccessManagerData accessData);
    
    Task<AccessManagerData> LoadAccessData();
    
    Task SaveTravelAsync(Travel travel);
    
    Task SaveAllTravelsAsync(IEnumerable<Travel> travels);
    
    Task<IEnumerable<Travel>> LoadAllTravelsAsync();
}