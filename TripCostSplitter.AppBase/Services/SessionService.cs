using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.AppBase.Services;

public class SessionService
{
    public Travel? CurrentTravel { get; set; }
    public Transaction? CurrentTransaction { get; set; }
    
    private IDataService dataService;
    
    public SessionService(IDataService _dataService)
    {
        dataService = _dataService;
    }
    
    public async Task Save()
    {
        if (CurrentTravel != null)
            await dataService.SaveTravelAsync(CurrentTravel);
    }
}