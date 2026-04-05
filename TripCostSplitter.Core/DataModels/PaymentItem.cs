namespace TripCostSplitter.Core.DataModels;

public class PaymentItem
{
    public required string Item { get; set; }
    public required decimal Amount { get; set; }
}