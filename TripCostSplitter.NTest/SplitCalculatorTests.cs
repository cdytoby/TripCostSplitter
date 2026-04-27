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
        
        _alice = new Person("1", "Alice");
        _bob = new Person("2", "Bob");
        _charlie = new Person("3", "Charlie");
    }
    
    // Test case: Single payer, single participant, amount should be divided evenly
    [Test]
    public void SplitEvenlyCalculator_CalculateDebit_SinglePayer_SingleParticipant_ReturnsCorrectAmount()
    {
        // Arrange
        SplitEvenly splitData = new();
        
        PaymentData paymentData = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            ParticipantIds = [_alice.Id],
            PayerInfos = 
            [
                new PayerInfo(_alice.Id, 100)
            ],
            PurchaseItems = [],
            SplitData = splitData
        };
        
        // Act
        IList<RecipientInfo> result = _evenlyCalculator.CalculateDebit(paymentData);
        
        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].RecipientId, Is.EqualTo(_alice.Id));
        Assert.That(result[0].Amount, Is.EqualTo(100));
    }
    
    // Test case: Single payer, multiple participants, amount should be split equally
    [Test]
    public void SplitEvenlyCalculator_CalculateDebit_SinglePayer_MultipleParticipants_ReturnsCorrectSplit()
    {
        // Arrange
        SplitEvenly splitData = new();
        
        PaymentData paymentData = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            ParticipantIds = 
            [
                _alice.Id,
                _bob.Id
            ],
            PayerInfos = 
            [
                new PayerInfo(_alice.Id, 100)
            ],
            PurchaseItems = [],
            SplitData = splitData
        };
        
        // Act
        IList<RecipientInfo> result = _evenlyCalculator.CalculateDebit(paymentData);
        
        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Sum(di => di.Amount), Is.EqualTo(100));
        Assert.That(result.First(di => di.RecipientId == _alice.Id).Amount, Is.EqualTo(50));
        Assert.That(result.First(di => di.RecipientId == _bob.Id).Amount, Is.EqualTo(50));
    }
    
    // Test case: Multiple payers, amount should be split evenly among all participants
    [Test]
    public void SplitEvenlyCalculator_CalculateDebit_MultiplePayers_ReturnsCorrectTotalSplit()
    {
        // Arrange
        SplitEvenly splitData = new();
        
        PaymentData paymentData = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            ParticipantIds = 
            [
                _alice.Id,
                _bob.Id,
                _charlie.Id
            ],
            PayerInfos = 
            [
                new PayerInfo(_alice.Id, 60),
                
                new PayerInfo(_bob.Id, 60)
            ],
            PurchaseItems = [],
            SplitData = splitData
        };
        
        // Act
        IList<RecipientInfo> result = _evenlyCalculator.CalculateDebit(paymentData);
        
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
        
        PaymentData paymentData = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            ParticipantIds = 
            [
                _alice.Id,
                _bob.Id,
                _charlie.Id
            ],
            PayerInfos = 
            [
                new PayerInfo(_alice.Id, totalAmount)
            ],
            PurchaseItems = [],
            SplitData = splitData
        };
        
        // Act
        IList<RecipientInfo> result = _evenlyCalculator.CalculateDebit(paymentData);
        
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
            PersonPercentageDict = new Dictionary<string, decimal>
            {
                { _alice.Id, 100 }
            }
        };
        
        PaymentData paymentData = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            ParticipantIds = [_alice.Id],
            PayerInfos = 
            [
                new PayerInfo(_alice.Id, 100)
            ],
            PurchaseItems = [],
            SplitData = splitData
        };
        
        // Act
        IList<RecipientInfo> result = _percentageCalculator.CalculateDebit(paymentData);
        
        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].RecipientId, Is.EqualTo(_alice.Id));
        Assert.That(result[0].Amount, Is.EqualTo(100));
    }
    
    // Test case: Multiple participants with different percentages should be split accordingly
    [Test]
    public void SplitByPercentageCalculator_CalculateDebit_MultipleParticipants_DifferentPercentages()
    {
        // Arrange
        SplitByPercentage splitData = new()
        {
            PersonPercentageDict = new Dictionary<string, decimal>
            {
                { _alice.Id, 50 },
                { _bob.Id, 30 },
                { _charlie.Id, 20 }
            }
        };
        
        PaymentData paymentData = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            ParticipantIds = 
            [
                _alice.Id,
                _bob.Id,
                _charlie.Id
            ],
            PayerInfos = 
            [
                new PayerInfo(_alice.Id, 100)
            ],
            PurchaseItems = [],
            SplitData = splitData
        };
        
        // Act
        IList<RecipientInfo> result = _percentageCalculator.CalculateDebit(paymentData);
        
        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result.Sum(di => di.Amount), Is.EqualTo(100));
        Assert.That(result.First(di => di.RecipientId == _alice.Id).Amount, Is.EqualTo(50));
        Assert.That(result.First(di => di.RecipientId == _bob.Id).Amount, Is.EqualTo(30));
        Assert.That(result.First(di => di.RecipientId == _charlie.Id).Amount, Is.EqualTo(20));
    }
    
    // Test case: TotalExactValidation enabled with percentages that don't sum to 100
    [Test]
    public void SplitByPercentageCalculator_CalculateDebit_WithTotalExactValidation_AdjustsLastAmount()
    {
        // Arrange
        SplitByPercentage splitData = new()
        {
            PersonPercentageDict = new Dictionary<string, decimal>
            {
                { _alice.Id, 50 },
                { _bob.Id, 30 },
                { _charlie.Id, 19 }
            },
            TotalExactValidation = true
        };
        
        PaymentData paymentData = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            ParticipantIds = 
            [
                _alice.Id,
                _bob.Id,
                _charlie.Id
            ],
            PayerInfos = 
            [
                new PayerInfo(_alice.Id, 100)
            ],
            PurchaseItems = [],
            SplitData = splitData
        };
        
        // Act
        IList<RecipientInfo> result = _percentageCalculator.CalculateDebit(paymentData);
        
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
            PersonIdAmountDict = new Dictionary<string, decimal>
            {
                { _alice.Id, 50 },
                { _bob.Id, 30 },
                { _charlie.Id, 20 }
            }
        };
        
        PaymentData paymentData = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            ParticipantIds = 
            [
                _alice.Id,
                _bob.Id,
                _charlie.Id
            ],
            PayerInfos = 
            [
                new PayerInfo(_alice.Id, 100)
            ],
            PurchaseItems = [],
            SplitData = splitData
        };
        
        // Act
        IList<RecipientInfo> result = _exactAmountCalculator.CalculateDebit(paymentData);
        
        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result.First(di => di.RecipientId == _alice.Id).Amount, Is.EqualTo(50));
        Assert.That(result.First(di => di.RecipientId == _bob.Id).Amount, Is.EqualTo(30));
        Assert.That(result.First(di => di.RecipientId == _charlie.Id).Amount, Is.EqualTo(20));
    }
    
    // Test case: Person who owns an item gets the cost of that item
    [Test]
    public void SplitByItemOwnershipCalculator_CalculateDebit_PersonOwnsItem_ReceivesItemCost()
    {
        // Arrange
        SplitByItemOwnership splitData = new()
        {
            OwnershipGroups = new Dictionary<string, List<string>?>
            {
                {
                    _bob.Id, ["Apple"]
                },
                {
                    _charlie.Id, ["Orange"]
                }
            }
        };
        
        PaymentData paymentData = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            ParticipantIds = 
            [
                _alice.Id,
                _bob.Id,
                _charlie.Id
            ],
            PayerInfos = 
            [
                new PayerInfo(_alice.Id, 100)
            ],
            PurchaseItems = 
            [
                new PurchaseItem("Apple", 60),
                new PurchaseItem("Orange", 40)
            ],
            SplitData = splitData
        };
        
        // Act
        IList<RecipientInfo> result = _ownershipCalculator.CalculateDebit(paymentData);
        
        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result.First(di => di.RecipientId == _bob.Id).Amount, Is.EqualTo(60));
        Assert.That(result.First(di => di.RecipientId == _charlie.Id).Amount, Is.EqualTo(40));
        Assert.That(result.First(di => di.RecipientId == _alice.Id).Amount, Is.EqualTo(0));
    }
    
    // Test case: Alice owns two items, should receive sum of both item costs
    [Test]
    public void SplitByItemOwnershipCalculator_CalculateDebit_PersonOwnsMultipleItems_SumsItemCosts()
    {
        // Arrange
        SplitByItemOwnership splitData = new()
        {
            OwnershipGroups = new Dictionary<string, List<string>?>
            {
                {
                    _bob.Id, ["Groceries"]
                },
                {
                    _charlie.Id, 
                    [
                        "Gas",
                        "Parking"
                    ]
                }
            }
        };
        
        PaymentData paymentData = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            ParticipantIds = 
            [
                _alice.Id,
                _bob.Id,
                _charlie.Id
            ],
            PayerInfos = 
            [
                new PayerInfo(_alice.Id, 100)
            ],
            PurchaseItems = 
            [
                new PurchaseItem("Groceries", 60),
                new PurchaseItem("Gas", 20),
                new PurchaseItem("Parking", 10),
                new PurchaseItem("Hotel", 10)
            ],
            SplitData = splitData
        };
        
        // Act
        IList<RecipientInfo> result = _ownershipCalculator.CalculateDebit(paymentData);
        
        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result.First(di => di.RecipientId == _bob.Id).Amount, Is.EqualTo(60));
        Assert.That(result.First(di => di.RecipientId == _charlie.Id).Amount, Is.EqualTo(30));
    }
}