using TripCostSplitter.Core;

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
            PayerInfos = new List<PayerInfo>
            {
                new() { Payer = alice, Amount = 100, Currency = "USD" }
            },
            DebitInfos = new List<DebitInfo>
            {
                new() { Payee = alice, Amount = 50, Currency = "USD" },
                new() { Payee = bob, Amount = 50, Currency = "USD" }
            },
            PaymentItems = new List<PaymentItem>()
        };

        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Payments = new List<IPayment> { payment }
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
            PayerInfos = new List<PayerInfo>
            {
                new() { Payer = alice, Amount = 60, Currency = "USD" }
            },
            DebitInfos = new List<DebitInfo>
            {
                new() { Payee = alice, Amount = 30, Currency = "USD" },
                new() { Payee = bob, Amount = 30, Currency = "USD" }
            },
            PaymentItems = new List<PaymentItem>()
        };

        Payment payment2 = new()
        {
            Date = DateTime.Now,
            PayerInfos = new List<PayerInfo>
            {
                new() { Payer = bob, Amount = 40, Currency = "USD" }
            },
            DebitInfos = new List<DebitInfo>
            {
                new() { Payee = alice, Amount = 20, Currency = "USD" },
                new() { Payee = bob, Amount = 20, Currency = "USD" }
            },
            PaymentItems = new List<PaymentItem>()
        };

        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Payments = new List<IPayment> { payment1, payment2 }
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
            PayerInfos = new List<PayerInfo>
            {
                new() { Payer = alice, Amount = 150, Currency = "USD" }
            },
            DebitInfos = new List<DebitInfo>
            {
                new() { Payee = alice, Amount = 50, Currency = "USD" },
                new() { Payee = bob, Amount = 50, Currency = "USD" },
                new() { Payee = charlie, Amount = 50, Currency = "USD" }
            },
            PaymentItems = new List<PaymentItem>()
        };

        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Payments = new List<IPayment> { payment }
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
            PayerInfos = new List<PayerInfo>
            {
                new() { Payer = alice, Amount = 100, Currency = "USD" }
            },
            DebitInfos = new List<DebitInfo>
            {
                new() { Payee = alice, Amount = 100, Currency = "USD" }
            },
            PaymentItems = new List<PaymentItem>()
        };

        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Payments = new List<IPayment> { payment }
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
            PayerInfos = new List<PayerInfo>
            {
                new() { Payer = alice, Amount = 120, Currency = "USD" }
            },
            DebitInfos = new List<DebitInfo>
            {
                new() { Payee = alice, Amount = 30, Currency = "USD" },
                new() { Payee = bob, Amount = 30, Currency = "USD" },
                new() { Payee = charlie, Amount = 30, Currency = "USD" },
                new() { Payee = dave, Amount = 30, Currency = "USD" }
            },
            PaymentItems = new List<PaymentItem>()
        };

        Payment payment2 = new()
        {
            Date = DateTime.Now,
            PayerInfos = new List<PayerInfo>
            {
                new() { Payer = bob, Amount = 80, Currency = "USD" }
            },
            DebitInfos = new List<DebitInfo>
            {
                new() { Payee = bob, Amount = 40, Currency = "USD" },
                new() { Payee = charlie, Amount = 40, Currency = "USD" }
            },
            PaymentItems = new List<PaymentItem>()
        };

        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Payments = new List<IPayment> { payment1, payment2 }
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
            PayerInfos = new List<PayerInfo>
            {
                new() { Payer = alice, Amount = 60, Currency = "USD" },
                new() { Payer = bob, Amount = 60, Currency = "USD" }
            },
            DebitInfos = new List<DebitInfo>
            {
                new() { Payee = alice, Amount = 40, Currency = "USD" },
                new() { Payee = bob, Amount = 40, Currency = "USD" },
                new() { Payee = charlie, Amount = 40, Currency = "USD" }
            },
            PaymentItems = new List<PaymentItem>()
        };

        Travel travel = new()
        {
            Name = "Trip",
            CalculateCurrency = "USD",
            Payments = new List<IPayment> { payment }
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
            Payments = new List<IPayment>()
        };

        // Act
        List<DebtItem> debts = _calculator.CalculateDebts(travel).ToList();

        // Assert
        Assert.That(debts, Is.Empty);
    }
}