using System.Text.Json;
using TripCostSplitter.Core;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.NTest;

[TestFixture]
public class TravelSerializationTests
{
    [Test]
    public void Travel_SerializationRoundTrip_ShouldPreserveData()
    {
        // Arrange
        Person alice = new("1", "Alice");
        Person bob = new("2", "Bob");
        
        PaymentData paymentData = new()
        {
            ParticipantIds = [alice.Id, bob.Id],
            PayerInfos = [new PayerInfo(alice.Id, 100)],
            PurchaseItems =
            [
                new PurchaseItem("Apple", 60),
                new PurchaseItem("Orange", 40)
            ],
            SplitData = new SplitByItemOwnership
            {
                OwnershipGroups = new Dictionary<string, List<string>>
                {
                    { alice.Id, ["Apple", "Orange"] }
                }
            }
        };
        
        Transaction transaction = new()
        {
            TransactionId = "1234",
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            Currency = "USD",
            TransactionData = paymentData,
            RecipientInfos =
            [
                new RecipientInfo(alice.Id, 50),
                new RecipientInfo(bob.Id, 50)
            ]
        };
        
        Travel originalTravel = new()
        {
            TravelId = "5678",
            Name = "Trip",
            CalculateCurrency = "USD",
            Transactions = [transaction]
        };
        
        // Act
        string jsonString = JsonSerializer.Serialize(originalTravel, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        Console.WriteLine(jsonString);
        
        Travel? deserializedTravel = JsonSerializer.Deserialize<Travel>(jsonString);
        
        // Assert
        Assert.That(deserializedTravel!.Transactions.First().TransactionData is PaymentData);
        Assert.That(deserializedTravel!.Transactions.First().TransactionId.Equals("1234"));
    }
    
    [Test]
    public void GUIDTest()
    {
        Console.WriteLine(Guid.NewGuid());
    }
}