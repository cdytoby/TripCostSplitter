namespace TripCostSplitter.Core.DataModels;

public class PaymentItem
{
    public string? Item { get; set; }
    public required decimal Amount { get; set; }
    public required string Currency { get; set; }
}