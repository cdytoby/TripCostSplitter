namespace TripCostSplitter.Core;

public class Travel
{
    public required string Name { get; set; }
    public required string CalculateCurrency { get; set; }
    public required IList<IPayment> Payments { get; set; }
}