using Microsoft.Extensions.DependencyInjection;
using TripCostSplitter.Core;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.NTest;

public class DebtCalculatorExchangeTest
{
    private DebtCalculator _calculator = null!;
    
    [SetUp]
    public void Setup()
    {
        _calculator = TestSetup.ServiceProvider.GetService<DebtCalculator>()!;
    }
    
    [Test]
    public void CalculateDebts_ExchangeRateOverride_UsesOverride()
    {
        // Arrange
        Person alice = new("1", "Alice");
        Person bob = new("2", "Bob");
        
        PaymentData paymentData = new()
        {
            ParticipantIds = [alice.Id, bob.Id],
            PayerInfos = [new PayerInfo(alice.Id, 100)],
            PurchaseItems = []
        };
        
        Transaction transaction = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            TransactionData = paymentData,
            Currency = "EUR",
            ExchangeRateOverride = 1.2m, // 1 EUR = 1.2 USD
            RecipientInfos =
            [
                new RecipientInfo(alice.Id, 50),
                new RecipientInfo(bob.Id, 50)
            ]
        };
        
        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Transactions = [transaction]
        };
        
        // Act
        List<DebtItem> debts = _calculator.CalculateDebts(travel).ToList();
        
        // Assert
        Assert.That(debts.Single().DebtorId, Is.EqualTo(bob.Id));
        Assert.That(debts.Single().CreditorId, Is.EqualTo(alice.Id));
        // 50 EUR * 1.2 = 60 USD
        Assert.That(debts.Single().Amount, Is.EqualTo(60m));
    }
    
    [Test]
    public void CalculateDebts_UsingServiceExchangeRate_CalculatesCorrectly()
    {
        // Arrange
        Person alice = new("1", "Alice");
        Person bob = new("2", "Bob");
        
        PaymentData paymentData = new()
        {
            ParticipantIds = [alice.Id, bob.Id],
            PayerInfos = [new PayerInfo(alice.Id, 100)],
            PurchaseItems = []
        };
        
        Transaction transaction = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            TransactionData = paymentData,
            Currency = "USD",
            RecipientInfos =
            [
                new RecipientInfo(alice.Id, 50),
                new RecipientInfo(bob.Id, 50)
            ]
        };
        
        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "EUR",
            Transactions = [transaction]
        };
        
        // MockDataService has USD -> EUR = 0.9
        
        // Act
        List<DebtItem> debts = _calculator.CalculateDebts(travel).ToList();
        
        // Assert
        Assert.That(debts.Single().DebtorId, Is.EqualTo(bob.Id));
        Assert.That(debts.Single().Amount, Is.EqualTo(50m * 0.9m).Within(0.0001m));
    }
    
    [Test]
    public void CalculateDebts_ReverseExchangeRate_CalculatesCorrectly()
    {
        // Arrange
        Person alice = new("1", "Alice");
        Person bob = new("2", "Bob");
        
        PaymentData paymentData = new()
        {
            ParticipantIds = [alice.Id, bob.Id],
            PayerInfos = [new PayerInfo(alice.Id, 90)],
            PurchaseItems = []
        };
        
        Transaction transaction = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            TransactionData = paymentData,
            Currency = "EUR", // Travel is in USD
            RecipientInfos =
            [
                new RecipientInfo(alice.Id, 45),
                new RecipientInfo(bob.Id, 45)
            ]
        };
        
        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Transactions = [transaction]
        };
        
        // MockDataService has USD -> EUR = 0.9
        // So EUR -> USD = 1 / 0.9 = 1.111...
        // 45 EUR * (1 / 0.9) = 50 USD
        
        // Act
        List<DebtItem> debts = [];
        for (int i = 0; i < 20; i++)
        {
            debts = _calculator.CalculateDebts(travel).ToList();
            if (debts.Count > 0 && debts[0].Amount > 0) break;
            Thread.Sleep(100);
        }
        
        // Assert
        Assert.That(debts.Single().Amount, Is.EqualTo(50m).Within(0.0001m));
    }
    
    [Test]
    public void CalculateDebts_MixedCurrencies_CalculatesCorrectly()
    {
        // Arrange
        Person alice = new("1", "Alice");
        Person bob = new("2", "Bob");
        
        // t1: Alice pays 100 USD (Travel currency)
        Transaction t1 = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            Currency = "USD",
            TransactionData = new PaymentData
            {
                ParticipantIds = [alice.Id, bob.Id],
                PayerInfos = [new PayerInfo(alice.Id, 100)],
                PurchaseItems = []
            },
            RecipientInfos = [new RecipientInfo(alice.Id, 50), new RecipientInfo(bob.Id, 50)]
        };
        
        // t2: Bob pays 100 EUR (USD->EUR = 0.9, so 100 EUR = 111.111 USD)
        Transaction t2 = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            Currency = "EUR",
            TransactionData = new PaymentData
            {
                ParticipantIds = [alice.Id, bob.Id],
                PayerInfos = [new PayerInfo(bob.Id, 100)],
                PurchaseItems = []
            },
            RecipientInfos = [new RecipientInfo(alice.Id, 50), new RecipientInfo(bob.Id, 50)]
        };
        
        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Transactions = [t1, t2]
        };
        
        // Act
        List<DebtItem> debts = _calculator.CalculateDebts(travel).ToList();
        
        // Assert
        Assert.That(debts.Single().DebtorId, Is.EqualTo(alice.Id));
        Assert.That(debts.Single().Amount, Is.EqualTo(5.55555m).Within(0.0001m));
    }
}