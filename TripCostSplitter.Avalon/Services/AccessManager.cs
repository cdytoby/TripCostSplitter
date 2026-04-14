using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.Avalon.Services;

public class AccessManager
{
    private IDataService dataService;
    private AccessManagerData accessData;
    
    public AccessManager(IDataService _dataService)
    {
        dataService = _dataService;
        accessData = _dataService.LoadAccessData().Result;
    }
    
    public int GetNextId()
    {
        int result = accessData.NextId;
        accessData.NextId++;
        dataService.SaveAccessData(accessData);
        return result;
    }
}