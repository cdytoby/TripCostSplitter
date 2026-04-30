using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.AppBase.Services;

public class SessionService
{
    public Travel? CurrentTravel { get; set; }
    public Transaction? CurrentTransaction { get; set; }
    
    private AccessManager accessManager;
    
    public SessionService(AccessManager _accessManager)
    {
        accessManager = _accessManager;
    }
    
    public IEnumerable<Travel> GetAllTravels()
    {
        return accessManager.GetAllTravels();
    }
    
    public async Task Save()
    {
        if (CurrentTravel != null)
            await accessManager.SaveTravel(CurrentTravel);
    }
    
    public async Task DeleteTravel(Travel travelToDelete)
    {
        await accessManager.DeleteTravel(travelToDelete.TravelId);
    }
}