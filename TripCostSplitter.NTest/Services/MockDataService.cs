using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.NTest.Services;

public class MockDataService: IDataService
{
    public IEnumerable<Travel> Travels { get; } = [];
    
    public SettingsDataModel Settings { get; } = new()
    {
        DefaultCurrency = "USD",
        CachedExchangeRates = new List<CurrencyExchangeRateModel>
        {
            new("USD", "EUR", 0.9m),
            new("EUR", "JPY", 150m)
        }
    };
    
    private Dictionary<string, Travel> travelsDict = new();
    
    public Task Load()
    {
        return Task.CompletedTask;
    }
    
    public Travel? GetTravel(string travelId)
    {
        return travelsDict.GetValueOrDefault(travelId);
    }
    
    public Task SaveTravelAsync(Travel newTravel)
    {
        return Task.CompletedTask;
    }
    
    public Task SaveAllTravelsAsync()
    {
        return Task.CompletedTask;
    }
    
    public Task DeleteTravelAsync(string travelId)
    {
        return Task.CompletedTask;
    }
    
    public Task SaveSettingsAsync()
    {
        return Task.CompletedTask;
    }
}