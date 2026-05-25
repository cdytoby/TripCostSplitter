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
    
    public async Task SaveTravel()
    {
        if (CurrentTravel != null)
            await accessManager.SaveTravel(CurrentTravel);
    }
    
    public async Task SaveTransaction(Transaction modifiedTransaction)
    {
        if (CurrentTravel == null)
            return;
        
        int position = CurrentTravel.Transactions.IndexOf(
            CurrentTravel.Transactions.Single(t => t.TransactionId.Equals(modifiedTransaction.TransactionId)));
        CurrentTravel.Transactions[position] = modifiedTransaction;
        await SaveTravel();
    }
    
    public async Task DeleteTravel(Travel travelToDelete)
    {
        await accessManager.DeleteTravel(travelToDelete.TravelId);
    }
}