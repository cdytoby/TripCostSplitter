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
}