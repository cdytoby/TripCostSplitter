using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.NTest.Services;

public class MockDataService: IDataService
{
    public Task SaveTravelAsync(Travel travel)
    {
        return Task.CompletedTask;
    }
    
    public Task SaveAllTravelsAsync(IEnumerable<Travel> travels)
    {
        return Task.CompletedTask;
    }
    
    public async Task<IEnumerable<Travel>> LoadAllTravelsAsync()
    {
        return [];
    }
    
    public Task DeleteTravelAsync(Travel travel)
    {
        return Task.CompletedTask;
    }
    
    public async Task<SettingsDataModel> LoadSettingsAsync()
    {
        return new SettingsDataModel()
        {
            DefaultCurrency = "USD",
            CachedExchangeRates = new List<CurrencyExchangeRateModel>
            {
                new("USD", "EUR", 0.9m),
                new("EUR", "JPY", 150m)
            }
        };
    }
    
    public Task SaveSettingsAsync(SettingsDataModel settings)
    {
        return Task.CompletedTask;
    }
}