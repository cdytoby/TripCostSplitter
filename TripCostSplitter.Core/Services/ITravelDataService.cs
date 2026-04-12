using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.Services;

public interface ITravelDataService
{
    Task SaveAsync(Travel travel);
    
    Task SaveAllAsync(IEnumerable<Travel> travels);
    
    Task<IEnumerable<Travel>> LoadAllAsync();
}