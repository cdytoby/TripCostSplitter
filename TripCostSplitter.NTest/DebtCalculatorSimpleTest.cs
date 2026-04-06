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
        Person alice = new(1, "Alice");
        Person bob = new(2, "Bob");
        
        PaymentData paymentData = new()
        {
            Date = DateTime.Now,
            Participants = [alice, bob],
            Currency = "USD",
            PayerInfos = [new PayerInfo(alice, 100)],
            PurchaseItems = []
        };

        Transaction transaction = new()
        {
            TransactionData = paymentData,
            RecipientInfos = 
            [
                new RecipientInfo(alice, 50),
                new RecipientInfo(bob, 50)
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
        Assert.That(debts[0].Debtor, Is.EqualTo(bob));
        Assert.That(debts[0].Creditor, Is.EqualTo(alice));
        Assert.That(debts[0].Amount, Is.EqualTo(50));
    }
    
    [Test]
    public void CalculateDebts_MultiplePayments_CalculatesNetBalance()
    {
        // Arrange
        Person alice = new(1, "Alice");
        Person bob = new(2, "Bob");
        
        PaymentData payment1 = new()
        {
            Date = DateTime.Now,
            Participants = [alice, bob],
            Currency = "USD",
            PayerInfos = [new PayerInfo(alice, 60)],
            PurchaseItems = []
        };

        Transaction pwd1 = new()
        {
            TransactionData = payment1,
            RecipientInfos = 
            [
                new RecipientInfo(alice, 30),
                new RecipientInfo(bob, 30)
            ]
        };
        
        PaymentData payment2 = new()
        {
            Date = DateTime.Now,
            Participants = [alice, bob],
            Currency = "USD",
            PayerInfos = [new PayerInfo(bob, 40)],
            PurchaseItems = []
        };

        Transaction pwd2 = new()
        {
            TransactionData = payment2,
            RecipientInfos = 
            [
                new RecipientInfo(alice, 20),
                new RecipientInfo(bob, 20)
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
        Assert.That(debts[0].Debtor, Is.EqualTo(bob));
        Assert.That(debts[0].Creditor, Is.EqualTo(alice));
        Assert.That(debts[0].Amount, Is.EqualTo(10));
    }
    
    [Test]
    public void CalculateDebts_ThreePeople_OptimizesDebts()
    {
        // Arrange
        Person alice = new(1, "Alice");
        Person bob = new(2, "Bob");
        Person charlie = new(3, "Charlie");
        
        PaymentData paymentData = new()
        {
            Date = DateTime.Now,
            Participants = [alice, bob, charlie],
            Currency = "USD",
            PayerInfos = [new PayerInfo(alice, 150)],
            PurchaseItems = []
        };

        Transaction pwd = new()
        {
            TransactionData = paymentData,
            RecipientInfos = 
            [
                new RecipientInfo(alice, 50),
                new RecipientInfo(bob, 50),
                new RecipientInfo(charlie, 50)
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
        Assert.That(debts.All(d => d.Creditor == alice), Is.True);
    }
    
    [Test]
    public void CalculateDebts_NoDebts_ReturnsEmpty()
    {
        // Arrange
        Person alice = new(1, "Alice");
        
        PaymentData paymentData = new()
        {
            Date = DateTime.Now,
            Participants = [alice],
            Currency = "USD",
            PayerInfos = [new PayerInfo(alice, 100)],
            PurchaseItems = []
        };

        Transaction pwd = new()
        {
            TransactionData = paymentData,
            RecipientInfos = [new RecipientInfo(alice, 100)]
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
        Person alice = new(1, "Alice");
        Person bob = new(2, "Bob");
        Person charlie = new(3, "Charlie");
        Person dave = new(4, "Dave");
        
        PaymentData payment1 = new()
        {
            Date = DateTime.Now,
            Participants = [alice, bob, charlie, dave],
            Currency = "USD",
            PayerInfos = [new PayerInfo(alice, 120)],
            PurchaseItems = []
        };

        Transaction pwd1 = new()
        {
            TransactionData = payment1,
            RecipientInfos = 
            [
                new RecipientInfo(alice, 30),
                new RecipientInfo(bob, 30),
                new RecipientInfo(charlie, 30),
                new RecipientInfo(dave, 30)
            ]
        };
        
        PaymentData payment2 = new()
        {
            Date = DateTime.Now,
            Participants = [alice, bob, charlie, dave],
            Currency = "USD",
            PayerInfos = [new PayerInfo(bob, 80)],
            PurchaseItems = []
        };

        Transaction pwd2 = new()
        {
            TransactionData = payment2,
            RecipientInfos = 
            [
                new RecipientInfo(bob, 40),
                new RecipientInfo(charlie, 40)
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
        Assert.That(debts.Any(d => d.Debtor == charlie), Is.True);
        Assert.That(debts.Any(d => d.Debtor == dave), Is.True);
    }
    
    [Test]
    public void CalculateDebts_MultiplePayersOnOnePayment_HandlesCorrectly()
    {
        // Arrange
        Person alice = new(1, "Alice");
        Person bob = new(2, "Bob");
        Person charlie = new(3, "Charlie");
        
        PaymentData paymentData = new()
        {
            Date = DateTime.Now,
            Participants = [alice, bob, charlie],
            Currency = "USD",
            PayerInfos = 
            [
                new PayerInfo(alice, 60),
                new PayerInfo(bob, 60)
            ],
            PurchaseItems = []
        };

        Transaction pwd = new()
        {
            TransactionData = paymentData,
            RecipientInfos =
            [
                new RecipientInfo(alice, 40),
                new RecipientInfo(bob, 40),
                new RecipientInfo(charlie, 40)
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
        Assert.That(debts.All(d => d.Debtor == charlie), Is.True);
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