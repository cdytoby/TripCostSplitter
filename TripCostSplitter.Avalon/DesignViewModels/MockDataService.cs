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
                    new Person("2", "Bob"),
                    new Person("3", "Charlie")
                ],
                Transactions =
                [
                    new Transaction
                    {
                        Description = "Transaction 1",
                        Date = DateTime.Now - TimeSpan.FromDays(5),
                        DateTimeZone = TimeZoneInfo.Local,
                        Currency = "EUR",
                        TransactionData = new PaymentData
                        {
                            ParticipantIds = ["1", "2", "3"],
                            PayerInfos = [new PayerInfo("1", 100)],
                            PurchaseItems = [new PurchaseItem("ItemToPurchase", 100)]
                        },
                        RecipientInfos =
                        [
                            new RecipientInfo("1", 50),
                            new RecipientInfo("2", 20),
                            new RecipientInfo("3", 30)
                        ]
                    },
                    new Transaction
                    {
                        Description = "Transaction 2 Lorem ipsum dolor sit amet, consectetur adipiscing ",
                        Date = DateTime.Now - TimeSpan.FromDays(2) + TimeSpan.FromHours(14),
                        DateTimeZone = TimeZoneInfo.Local,
                        Currency = "USD",
                        ExchangeRateOverride = 0.83m,
                        TransactionData = new PaymentData
                        {
                            ParticipantIds = ["1", "2"],
                            PayerInfos = [new PayerInfo("1", 100)],
                            PurchaseItems = [
                                new PurchaseItem("Pasta", 15),
                                new PurchaseItem("Apple juice", 4),
                                new PurchaseItem("Pizza", 12),
                                new PurchaseItem("Orange juice", 4),
                                new PurchaseItem("Chicken Wings", 15),
                                new PurchaseItem("Tiramisu", 10),
                                new PurchaseItem("Steak", 30),
                                new PurchaseItem("Tip", 10),
                            ]
                        },
                        RecipientInfos =
                        [
                            new RecipientInfo("1", 50),
                            new RecipientInfo("2", 50)
                        ]
                    }
                ]
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