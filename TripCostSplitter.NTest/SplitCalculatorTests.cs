using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.NTest;

public class SplitCalculatorTests
{
    private SplitEvenlyCalculator _evenlyCalculator = null!;
    private SplitByPercentageCalculator _percentageCalculator = null!;
    private SplitByExactAmountCalculator _exactAmountCalculator = null!;
    private SplitByItemOwnershipCalculator _ownershipCalculator = null!;
    
    private Person _alice = null!;
    private Person _bob = null!;
    private Person _charlie = null!;
    
    [SetUp]
    public void Setup()
    {
        _evenlyCalculator = new SplitEvenlyCalculator();
        _percentageCalculator = new SplitByPercentageCalculator();
        _exactAmountCalculator = new SplitByExactAmountCalculator();
        _ownershipCalculator = new SplitByItemOwnershipCalculator();
        
        _alice = new Person(1, "Alice");
        _bob = new Person(2, "Bob");
        _charlie = new Person(3, "Charlie");
    }
    
    // Test case: Single payer, single participant, amount should be divided evenly
    [Test]
    public void SplitEvenlyCalculator_CalculateDebit_SinglePayer_SingleParticipant_ReturnsCorrectAmount()
    {
        // Arrange
        SplitEvenly splitData = new();
        
        Payment payment = new()
        {
            Date = DateTime.Now,
            Currency = "USD",
            Participants = (List<Person>)[_alice],
            PayerInfos = (List<PayerInfo>)
            [
                new PayerInfo
                {
                    Payer = _alice,
                    Amount = 100
                }
            ],
            PaymentItems = (List<PaymentItem>)[],
            SplitData = splitData
        };
        
        // Act
        IList<DebitInfo> result = _evenlyCalculator.CalculateDebit(payment);
        
        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Payee, Is.EqualTo(_alice));
        Assert.That(result[0].Amount, Is.EqualTo(100));
    }
    
    // Test case: Single payer, multiple participants, amount should be split equally
    [Test]
    public void SplitEvenlyCalculator_CalculateDebit_SinglePayer_MultipleParticipants_ReturnsCorrectSplit()
    {
        // Arrange
        SplitEvenly splitData = new();
        
        Payment payment = new()
        {
            Date = DateTime.Now,
            Currency = "USD",
            Participants = (List<Person>)
            [
                _alice,
                _bob
            ],
            PayerInfos = (List<PayerInfo>)
            [
                new PayerInfo
                {
                    Payer = _alice,
                    Amount = 100
                }
            ],
            PaymentItems = (List<PaymentItem>)[],
            SplitData = splitData
        };
        
        // Act
        IList<DebitInfo> result = _evenlyCalculator.CalculateDebit(payment);
        
        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Sum(di => di.Amount), Is.EqualTo(100));
        Assert.That(result.First(di => di.Payee == _alice).Amount, Is.EqualTo(50));
        Assert.That(result.First(di => di.Payee == _bob).Amount, Is.EqualTo(50));
    }
    
    // Test case: Multiple payers, amount should be split evenly among all participants
    [Test]
    public void SplitEvenlyCalculator_CalculateDebit_MultiplePayers_ReturnsCorrectTotalSplit()
    {
        // Arrange
        SplitEvenly splitData = new();
        
        Payment payment = new()
        {
            Date = DateTime.Now,
            Currency = "USD",
            Participants = (List<Person>)
            [
                _alice,
                _bob,
                _charlie
            ],
            PayerInfos = (List<PayerInfo>)
            [
                new PayerInfo
                {
                    Payer = _alice,
                    Amount = 60
                },
                
                new PayerInfo
                {
                    Payer = _bob,
                    Amount = 60
                }
            ],
            PaymentItems = (List<PaymentItem>)[],
            SplitData = splitData
        };
        
        // Act
        IList<DebitInfo> result = _evenlyCalculator.CalculateDebit(payment);
        
        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result.Sum(di => di.Amount), Is.EqualTo(120));
        Assert.That(result.All(di => Math.Abs(di.Amount - 40) < 0.01m));
    }
    
    // Test case: TotalExactValidation enabled, amount should sum to total paid
    [TestCase(101)]
    [TestCase(100)]
    public void SplitEvenlyCalculator_CalculateDebit_WithTotalExactValidation_AmountSumsCorrectly(decimal totalAmount)
    {
        // Arrange
        SplitEvenly splitData = new()
        {
            TotalExactValidation = true
        };
        
        Payment payment = new()
        {
            Date = DateTime.Now,
            Currency = "USD",
            Participants = (List<Person>)
            [
                _alice,
                _bob,
                _charlie
            ],
            PayerInfos = (List<PayerInfo>)
            [
                new PayerInfo
                {
                    Payer = _alice,
                    Amount = totalAmount
                }
            ],
            PaymentItems = (List<PaymentItem>)[],
            SplitData = splitData
        };
        
        // Act
        IList<DebitInfo> result = _evenlyCalculator.CalculateDebit(payment);
        
        // Assert
        Assert.That(result.Sum(di => di.Amount), Is.EqualTo(totalAmount));
    }
    
    // Test case: Single participant should receive 100% of the amount
    [Test]
    public void SplitByPercentageCalculator_CalculateDebit_SingleParticipant_OneHundredPercent()
    {
        // Arrange
        SplitByPercentage splitData = new()
        {
            PersonPercentageDict = new Dictionary<Person, decimal>
            {
                { _alice, 100 }
            }
        };
        
        Payment payment = new()
        {
            Date = DateTime.Now,
            Currency = "USD",
            Participants = (List<Person>)[_alice],
            PayerInfos = (List<PayerInfo>)
            [
                new PayerInfo
                {
                    Payer = _alice,
                    Amount = 100
                }
            ],
            PaymentItems = (List<PaymentItem>)[],
            SplitData = splitData
        };
        
        // Act
        IList<DebitInfo> result = _percentageCalculator.CalculateDebit(payment);
        
        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Payee, Is.EqualTo(_alice));
        Assert.That(result[0].Amount, Is.EqualTo(100));
    }
    
    // Test case: Multiple participants with different percentages should be split accordingly
    [Test]
    public void SplitByPercentageCalculator_CalculateDebit_MultipleParticipants_DifferentPercentages()
    {
        // Arrange
        SplitByPercentage splitData = new()
        {
            PersonPercentageDict = new Dictionary<Person, decimal>
            {
                { _alice, 50 },
                { _bob, 30 },
                { _charlie, 20 }
            }
        };
        
        Payment payment = new()
        {
            Date = DateTime.Now,
            Currency = "USD",
            Participants = (List<Person>)
            [
                _alice,
                _bob,
                _charlie
            ],
            PayerInfos = (List<PayerInfo>)
            [
                new PayerInfo
                {
                    Payer = _alice,
                    Amount = 100
                }
            ],
            PaymentItems = (List<PaymentItem>)[],
            SplitData = splitData
        };
        
        // Act
        IList<DebitInfo> result = _percentageCalculator.CalculateDebit(payment);
        
        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result.Sum(di => di.Amount), Is.EqualTo(100));
        Assert.That(result.First(di => di.Payee == _alice).Amount, Is.EqualTo(50));
        Assert.That(result.First(di => di.Payee == _bob).Amount, Is.EqualTo(30));
        Assert.That(result.First(di => di.Payee == _charlie).Amount, Is.EqualTo(20));
    }
    
    // Test case: TotalExactValidation enabled with percentages that don't sum to 100
    [Test]
    public void SplitByPercentageCalculator_CalculateDebit_WithTotalExactValidation_AdjustsLastAmount()
    {
        // Arrange
        SplitByPercentage splitData = new()
        {
            PersonPercentageDict = new Dictionary<Person, decimal>
            {
                { _alice, 50 },
                { _bob, 30 },
                { _charlie, 19 }
            },
            TotalExactValidation = true
        };
        
        Payment payment = new()
        {
            Date = DateTime.Now,
            Currency = "USD",
            Participants = (List<Person>)
            [
                _alice,
                _bob,
                _charlie
            ],
            PayerInfos = (List<PayerInfo>)
            [
                new PayerInfo
                {
                    Payer = _alice,
                    Amount = 100
                }
            ],
            PaymentItems = (List<PaymentItem>)[],
            SplitData = splitData
        };
        
        // Act
        IList<DebitInfo> result = _percentageCalculator.CalculateDebit(payment);
        
        // Assert
        Assert.That(result.Sum(di => di.Amount), Is.EqualTo(100));
    }
    
    // Test case: Each person receives exact amount specified
    [Test]
    public void SplitByExactAmountCalculator_CalculateDebit_EachPersonGetsExactAmount()
    {
        // Arrange
        SplitByExactAmount splitData = new()
        {
            PersonAmountDict = new Dictionary<Person, decimal>
            {
                { _alice, 50 },
                { _bob, 30 },
                { _charlie, 20 }
            }
        };
        
        Payment payment = new()
        {
            Date = DateTime.Now,
            Currency = "USD",
            Participants = (List<Person>)
            [
                _alice,
                _bob,
                _charlie
            ],
            PayerInfos = (List<PayerInfo>)
            [
                new PayerInfo
                {
                    Payer = _alice,
                    Amount = 100
                }
            ],
            PaymentItems = (List<PaymentItem>)[],
            SplitData = splitData
        };
        
        // Act
        IList<DebitInfo> result = _exactAmountCalculator.CalculateDebit(payment);
        
        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result.First(di => di.Payee == _alice).Amount, Is.EqualTo(50));
        Assert.That(result.First(di => di.Payee == _bob).Amount, Is.EqualTo(30));
        Assert.That(result.First(di => di.Payee == _charlie).Amount, Is.EqualTo(20));
    }
    
    // Test case: Person who owns an item gets the cost of that item
    [Test]
    public void SplitByItemOwnershipCalculator_CalculateDebit_PersonOwnsItem_ReceivesItemCost()
    {
        // Arrange
        SplitByItemOwnership splitData = new()
        {
            OwnershipGroups = new Dictionary<Person, IList<string>>
            {
                {
                    _bob, ["Apple"]
                },
                {
                    _charlie, ["Orange"]
                }
            }
        };
        
        Payment payment = new()
        {
            Date = DateTime.Now,
            Currency = "USD",
            Participants = (List<Person>)
            [
                _alice,
                _bob,
                _charlie
            ],
            PayerInfos = (List<PayerInfo>)
            [
                new PayerInfo
                {
                    Payer = _alice,
                    Amount = 100
                }
            ],
            PaymentItems = (List<PaymentItem>)
            [
                new PaymentItem
                {
                    Item = "Apple",
                    Amount = 60
                },
                
                new PaymentItem
                {
                    Item = "Orange",
                    Amount = 40
                }
            ],
            SplitData = splitData
        };
        
        // Act
        IList<DebitInfo> result = _ownershipCalculator.CalculateDebit(payment);
        
        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result.First(di => di.Payee == _bob).Amount, Is.EqualTo(60));
        Assert.That(result.First(di => di.Payee == _charlie).Amount, Is.EqualTo(40));
        Assert.That(result.First(di => di.Payee == _alice).Amount, Is.EqualTo(0));
    }
    
    // Test case: Alice owns two items, should receive sum of both item costs
    [Test]
    public void SplitByItemOwnershipCalculator_CalculateDebit_PersonOwnsMultipleItems_SumsItemCosts()
    {
        // Arrange
        SplitByItemOwnership splitData = new()
        {
            OwnershipGroups = new Dictionary<Person, IList<string>>
            {
                {
                    _bob, (List<string>)["Groceries"]
                },
                {
                    _charlie, (List<string>)
                    [
                        "Gas",
                        "Parking"
                    ]
                }
            }
        };
        
        Payment payment = new()
        {
            Date = DateTime.Now,
            Currency = "USD",
            Participants = (List<Person>)
            [
                _alice,
                _bob,
                _charlie
            ],
            PayerInfos = (List<PayerInfo>)
            [
                new PayerInfo
                {
                    Payer = _alice,
                    Amount = 100
                }
            ],
            PaymentItems = (List<PaymentItem>)
            [
                new PaymentItem
                {
                    Item = "Groceries",
                    Amount = 60
                },
                
                new PaymentItem
                {
                    Item = "Gas",
                    Amount = 20
                },
                
                new PaymentItem
                {
                    Item = "Parking",
                    Amount = 10
                },
                
                new PaymentItem
                {
                    Item = "Hotel",
                    Amount = 10
                }
            ],
            SplitData = splitData
        };
        
        // Act
        IList<DebitInfo> result = _ownershipCalculator.CalculateDebit(payment);
        
        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result.First(di => di.Payee == _bob).Amount, Is.EqualTo(60));
        Assert.That(result.First(di => di.Payee == _charlie).Amount, Is.EqualTo(30));
    }
}