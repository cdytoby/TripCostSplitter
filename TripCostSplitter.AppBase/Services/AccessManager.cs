using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.AppBase.Services;

public class AccessManager
{
    private readonly IDataService dataService;
    
    public AccessManager(IDataService _dataService)
    {
        dataService = _dataService;
    }
    
    public static string GetNewId()
    {
        return Guid.NewGuid().ToString();
    }
    
    public IEnumerable<Travel> GetAllTravels()
    {
        return dataService.Travels;
    }
    
    public async Task SaveTravel(Travel? travel)
    {
        if (travel != null)
            await dataService.SaveTravelAsync(travel);
    }
    
    public async Task DeleteTravel(string travelId)
    {
        await dataService.DeleteTravelAsync(travelId);
    }
}