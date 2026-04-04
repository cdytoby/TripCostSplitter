namespace TripCostSplitter.Core.DataModels;

public class DebitInfo
{
    public required Person Payee { get; set; }
    public required decimal Amount { get; set; }
    public required string Currency { get; set; }
}