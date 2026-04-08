using System.Collections.ObjectModel;
using TripCostSplitter.Avalon.Services;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.NTest;

public class TestTravelDataService : JsonTravelDataService
{
    public TestTravelDataService(string filePath) : base(filePath) { }
}

public class TravelDataServiceTest
{
    private string _testFilePath;

    [SetUp]
    public void SetUp()
    {
        _testFilePath = Path.Combine(AppContext.BaseDirectory, "travel_data.json");
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    [Test]
    public async Task SaveAndLoadTest()
    {
        var service = new TestTravelDataService(_testFilePath);
        var travels = new List<Travel>
        {
            new Travel
            {
                Name = "Test Trip",
                CalculateCurrency = "USD",
                Transactions = new ObservableCollection<Transaction>()
            }
        };

        await service.SaveAsync(travels);
        
        Assert.That(File.Exists(_testFilePath), Is.True);

        var loadedTravels = (await service.LoadAsync()).ToList();
        
        Assert.That(loadedTravels, Has.Count.EqualTo(1));
        Assert.That(loadedTravels[0].Name, Is.EqualTo("Test Trip"));
        Assert.That(loadedTravels[0].CalculateCurrency, Is.EqualTo("USD"));
    }

    [Test]
    public async Task LoadEmptyFileTest()
    {
        var service = new TestTravelDataService(_testFilePath);
        var loadedTravels = (await service.LoadAsync()).ToList();
        
        Assert.That(loadedTravels, Is.Empty);
    }
}
