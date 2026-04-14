using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.Services;

public interface IDataService
{
    public Task SaveAccessData(AccessManagerData accessData);
    
    public Task<AccessManagerData> LoadAccessData();
    
    Task SaveTravelAsync(Travel travel);
    
    Task SaveAllTravelsAsync(IEnumerable<Travel> travels);
    
    Task<IEnumerable<Travel>> LoadAllTravelsAsync();
}