using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core;

public interface ITravelDataService
{
    Task SaveAsync(IEnumerable<Travel> travels);
    Task<IEnumerable<Travel>> LoadAsync();
}
