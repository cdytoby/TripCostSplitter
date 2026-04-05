namespace TripCostSplitter.Core.DataModels;

public class PayerInfo
{
    public required Person Payer { get; set; }
    public required decimal Amount { get; set; }
}