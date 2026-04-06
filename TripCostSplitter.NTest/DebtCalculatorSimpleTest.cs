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
        
        Payment payment = new()
        {
            Date = DateTime.Now,
            Participants = [alice, bob],
            Currency = "USD",
            PayerInfos = new List<PayerInfo>
            {
                new PayerInfo(alice, 100)
            },
            DebitInfos = new List<DebitInfo>
            {
                new DebitInfo(alice, 50),
                new DebitInfo(bob, 50)
            },
            PurchaseItems = new List<PurchaseItem>()
        };
        
        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Payments = new List<Payment>
            {
                payment
            }
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
        
        Payment payment1 = new()
        {
            Date = DateTime.Now,
            Participants = [alice, bob],
            Currency = "USD",
            PayerInfos = new List<PayerInfo>
            {
                new PayerInfo(alice, 60)
            },
            DebitInfos = new List<DebitInfo>
            {
                new DebitInfo(alice, 30),
                new DebitInfo(bob, 30)
            },
            PurchaseItems = new List<PurchaseItem>()
        };
        
        Payment payment2 = new()
        {
            Date = DateTime.Now,
            Participants = [alice, bob],
            Currency = "USD",
            PayerInfos = new List<PayerInfo>
            {
                new PayerInfo(bob, 40)
            },
            DebitInfos = new List<DebitInfo>
            {
                new DebitInfo(alice, 20),
                new DebitInfo(bob, 20)
            },
            PurchaseItems = new List<PurchaseItem>()
        };
        
        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Payments = new List<Payment>
            {
                payment1,
                payment2
            }
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
        
        Payment payment = new()
        {
            Date = DateTime.Now,
            Participants = [alice, bob, charlie],
            Currency = "USD",
            PayerInfos = new List<PayerInfo>
            {
                new PayerInfo(alice, 150)
            },
            DebitInfos = new List<DebitInfo>
            {
                new DebitInfo(alice, 50),
                new DebitInfo(bob, 50),
                new DebitInfo(charlie, 50)
            },
            PurchaseItems = new List<PurchaseItem>()
        };
        
        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Payments = new List<Payment>
            {
                payment
            }
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
        
        Payment payment = new()
        {
            Date = DateTime.Now,
            Participants = [alice],
            Currency = "USD",
            PayerInfos = new List<PayerInfo>
            {
                new PayerInfo(alice, 100)
            },
            DebitInfos = new List<DebitInfo>
            {
                new DebitInfo(alice, 100)
            },
            PurchaseItems = new List<PurchaseItem>()
        };
        
        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Payments = new List<Payment>
            {
                payment
            }
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
        
        Payment payment1 = new()
        {
            Date = DateTime.Now,
            Participants = [alice, bob, charlie, dave],
            Currency = "USD",
            PayerInfos = new List<PayerInfo>
            {
                new PayerInfo(alice, 120)
            },
            DebitInfos = new List<DebitInfo>
            {
                new DebitInfo(alice, 30),
                new DebitInfo(bob, 30),
                new DebitInfo(charlie, 30),
                new DebitInfo(dave, 30)
            },
            PurchaseItems = new List<PurchaseItem>()
        };
        
        Payment payment2 = new()
        {
            Date = DateTime.Now,
            Participants = [alice, bob, charlie, dave],
            Currency = "USD",
            PayerInfos = new List<PayerInfo>
            {
                new PayerInfo(bob, 80)
            },
            DebitInfos = new List<DebitInfo>
            {
                new DebitInfo(bob, 40),
                new DebitInfo(charlie, 40)
            },
            PurchaseItems = new List<PurchaseItem>()
        };
        
        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Payments = new List<Payment>
            {
                payment1,
                payment2
            }
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
        
        Payment payment = new()
        {
            Date = DateTime.Now,
            Participants = [alice, bob, charlie],
            Currency = "USD",
            PayerInfos = new List<PayerInfo>
            {
                new PayerInfo(alice, 60),
                new PayerInfo(bob, 60)
            },
            DebitInfos = new List<DebitInfo>
            {
                new DebitInfo(alice, 40),
                new DebitInfo(bob, 40),
                new DebitInfo(charlie, 40)
            },
            PurchaseItems = new List<PurchaseItem>()
        };
        
        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Payments = new List<Payment>
            {
                payment
            }
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
            Payments = new List<Payment>()
        };
        
        // Act
        List<DebtItem> debts = _calculator.CalculateDebts(travel).ToList();
        
        // Assert
        Assert.That(debts, Is.Empty);
    }
}