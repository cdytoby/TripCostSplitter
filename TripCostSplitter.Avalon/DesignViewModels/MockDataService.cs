using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.Avalon.DesignViewModels;

public class MockDataService: IDataService
{
    public IEnumerable<Travel> Travels => travelsDict.Values;
    
    public SettingsDataModel Settings { get; } = new()
    {
        DefaultCurrency = "USD",
        CachedExchangeRates = new List<CurrencyExchangeRateModel>
        {
            new("USD", "EUR", 0.9m),
            new("EUR", "JPY", 150m)
        }
    };
    
    private Dictionary<string, Travel> travelsDict = new()
    {
        {
            "1", new Travel
            {
                TravelId = "1",
                Name = "MyTravel1",
                CalculateCurrency = "EUR",
                AdditionalCurrencies = ["USD", "JPY"],
                Participants =
                [
                    new Person("1", "Alice"),
                    new Person("2", "Bob")
                ],
            }
        },
        {
            "2", new Travel
            {
                TravelId = "2",
                Name = "MyTravel2",
                CalculateCurrency = "EUR",
                AdditionalCurrencies = ["USD"],
                Participants =
                [
                    new Person("1", "Alice"),
                    new Person("2", "Bob"),
                    new Person("3", "Charlie")
                ],
            }
        }
    };
    
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