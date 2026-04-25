using TripCostSplitter.Core;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.NTest;

public class DebtCalculatorSimpleTest
{
    private DebtCalculator _calculator = null!;
    
    [SetUp]
    public void Setup()
    {
        _calculator = new DebtCalculator();
    }
    
    [Test]
    public void CalculateDebts_SimpleOneToOne_ReturnsCorrectDebt()
    {
        // Arrange
        Person alice = new("1", "Alice");
        Person bob = new("2", "Bob");
        
        PaymentData paymentData = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            ParticipantIds = [alice.Id, bob.Id],
            Currency = "USD",
            PayerInfos = [new PayerInfo(alice.Id, 100)],
            PurchaseItems = []
        };

        Transaction transaction = new()
        {
            TransactionData = paymentData,
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
        Assert.That(debts, Has.Count.EqualTo(1));
        Assert.That(debts[0].DebtorId, Is.EqualTo(bob.Id));
        Assert.That(debts[0].CreditorId, Is.EqualTo(alice.Id));
        Assert.That(debts[0].Amount, Is.EqualTo(50));
    }
    
    [Test]
    public void CalculateDebts_MultiplePayments_CalculatesNetBalance()
    {
        // Arrange
        Person alice = new("1", "Alice");
        Person bob = new("2", "Bob");
        
        PaymentData payment1 = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            ParticipantIds = [alice.Id, bob.Id],
            Currency = "USD",
            PayerInfos = [new PayerInfo(alice.Id, 60)],
            PurchaseItems = []
        };

        Transaction pwd1 = new()
        {
            TransactionData = payment1,
            RecipientInfos = 
            [
                new RecipientInfo(alice.Id, 30),
                new RecipientInfo(bob.Id, 30)
            ]
        };
        
        PaymentData payment2 = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            ParticipantIds = [alice.Id, bob.Id],
            Currency = "USD",
            PayerInfos = [new PayerInfo(bob.Id, 40)],
            PurchaseItems = []
        };

        Transaction pwd2 = new()
        {
            TransactionData = payment2,
            RecipientInfos = 
            [
                new RecipientInfo(alice.Id, 20),
                new RecipientInfo(bob.Id, 20)
            ]
        };
        
        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Transactions = 
            [
                pwd1,
                pwd2
            ]
        };
        
        // Act
        List<DebtItem> debts = _calculator.CalculateDebts(travel).ToList();
        
        // Assert
        Assert.That(debts, Has.Count.EqualTo(1));
        Assert.That(debts[0].DebtorId, Is.EqualTo(bob.Id));
        Assert.That(debts[0].CreditorId, Is.EqualTo(alice.Id));
        Assert.That(debts[0].Amount, Is.EqualTo(10));
    }
    
    [Test]
    public void CalculateDebts_ThreePeople_OptimizesDebts()
    {
        // Arrange
        Person alice = new("1", "Alice");
        Person bob = new("2", "Bob");
        Person charlie = new("3", "Charlie");
        
        PaymentData paymentData = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            ParticipantIds = [alice.Id, bob.Id, charlie.Id],
            Currency = "USD",
            PayerInfos = [new PayerInfo(alice.Id, 150)],
            PurchaseItems = []
        };

        Transaction pwd = new()
        {
            TransactionData = paymentData,
            RecipientInfos = 
            [
                new RecipientInfo(alice.Id, 50),
                new RecipientInfo(bob.Id, 50),
                new RecipientInfo(charlie.Id, 50)
            ]
        };
        
        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Transactions = [pwd]
        };
        
        // Act
        List<DebtItem> debts = _calculator.CalculateDebts(travel).ToList();
        
        // Assert
        Assert.That(debts, Has.Count.EqualTo(2));
        Assert.That(debts.Sum(d => d.Amount), Is.EqualTo(100));
        Assert.That(debts.All(d => d.CreditorId == alice.Id), Is.True);
    }
    
    [Test]
    public void CalculateDebts_NoDebts_ReturnsEmpty()
    {
        // Arrange
        Person alice = new("1", "Alice");
        
        PaymentData paymentData = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            ParticipantIds = [alice.Id],
            Currency = "USD",
            PayerInfos = [new PayerInfo(alice.Id, 100)],
            PurchaseItems = []
        };

        Transaction pwd = new()
        {
            TransactionData = paymentData,
            RecipientInfos = [new RecipientInfo(alice.Id, 100)]
        };
        
        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Transactions = [pwd]
        };
        
        // Act
        List<DebtItem> debts = _calculator.CalculateDebts(travel).ToList();
        
        // Assert
        Assert.That(debts, Is.Empty);
    }
    
    [Test]
    public void CalculateDebts_ComplexScenario_CorrectlyBalances()
    {
        // Arrange
        Person alice = new("1", "Alice");
        Person bob = new("2", "Bob");
        Person charlie = new("3", "Charlie");
        Person dave = new("4", "Dave");
        
        PaymentData payment1 = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            ParticipantIds = [alice.Id, bob.Id, charlie.Id, dave.Id],
            Currency = "USD",
            PayerInfos = [new PayerInfo(alice.Id, 120)],
            PurchaseItems = []
        };

        Transaction pwd1 = new()
        {
            TransactionData = payment1,
            RecipientInfos = 
            [
                new RecipientInfo(alice.Id, 30),
                new RecipientInfo(bob.Id, 30),
                new RecipientInfo(charlie.Id, 30),
                new RecipientInfo(dave.Id, 30)
            ]
        };
        
        PaymentData payment2 = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            ParticipantIds = [alice.Id, bob.Id, charlie.Id, dave.Id],
            Currency = "USD",
            PayerInfos = [new PayerInfo(bob.Id, 80)],
            PurchaseItems = []
        };

        Transaction pwd2 = new()
        {
            TransactionData = payment2,
            RecipientInfos = 
            [
                new RecipientInfo(bob.Id, 40),
                new RecipientInfo(charlie.Id, 40)
            ]
        };
        
        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Transactions = 
            [
                pwd1,
                pwd2
            ]
        };
        
        // Act
        List<DebtItem> debts = _calculator.CalculateDebts(travel).ToList();
        
        // Assert
        decimal totalDebts = debts.Sum(d => d.Amount);
        Assert.That(totalDebts, Is.EqualTo(100m).Within(0.01m));
        Assert.That(debts.Any(d => d.DebtorId == charlie.Id), Is.True);
        Assert.That(debts.Any(d => d.DebtorId == dave.Id), Is.True);
    }
    
    [Test]
    public void CalculateDebts_MultiplePayersOnOnePayment_HandlesCorrectly()
    {
        // Arrange
        Person alice = new("1", "Alice");
        Person bob = new("2", "Bob");
        Person charlie = new("3", "Charlie");
        
        PaymentData paymentData = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            ParticipantIds = [alice.Id, bob.Id, charlie.Id],
            Currency = "USD",
            PayerInfos = 
            [
                new PayerInfo(alice.Id, 60),
                new PayerInfo(bob.Id, 60)
            ],
            PurchaseItems = []
        };

        Transaction pwd = new()
        {
            TransactionData = paymentData,
            RecipientInfos =
            [
                new RecipientInfo(alice.Id, 40),
                new RecipientInfo(bob.Id, 40),
                new RecipientInfo(charlie.Id, 40)
            ]
        };
        
        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Transactions = [pwd]
        };
        
        // Act
        List<DebtItem> debts = _calculator.CalculateDebts(travel).ToList();
        
        // Assert
        Assert.That(debts, Has.Count.EqualTo(2));
        Assert.That(debts.All(d => d.DebtorId == charlie.Id), Is.True);
        Assert.That(debts.Sum(d => d.Amount), Is.EqualTo(40));
    }
    
    [Test]
    public void CalculateDebts_EmptyPayments_ReturnsEmpty()
    {
        // Arrange
        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Transactions = []
        };
        
        // Act
        List<DebtItem> debts = _calculator.CalculateDebts(travel).ToList();
        
        // Assert
        Assert.That(debts, Is.Empty);
    }
}