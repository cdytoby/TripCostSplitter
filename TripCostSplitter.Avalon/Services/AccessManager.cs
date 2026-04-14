using System.Threading;
using System.Threading.Tasks;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.Avalon.Services;

public class AccessManager
{
    private readonly IDataService dataService;
    private readonly AccessManagerData accessData;
    private bool isSaving;
    private bool saveNeeded;
    
    public AccessManager(IDataService _dataService)
    {
        dataService = _dataService;
        //todo initialize async
        accessData = dataService.LoadAccessData().Result;
    }
    
    public int GetNextId()
    {
        int result = accessData.NextId;
        accessData.NextId++;
        
        TriggerSave();
        
        return result;
    }

    private void TriggerSave()
    {
        saveNeeded = true;
        if (isSaving)
        {
            return;
        }
        
        Task.Run(SaveInternal);
    }
    
    private async Task SaveInternal()
    {
        isSaving = true;
        AccessManagerData clone = accessData.Clone();
        while (saveNeeded)
        {
            await dataService.SaveAccessData(clone);
            saveNeeded = false;
        }
        
        isSaving = false;
    }
}