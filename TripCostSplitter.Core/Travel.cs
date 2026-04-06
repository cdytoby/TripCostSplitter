using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core;

public class Travel
{
    public required string Name { get; set; }
    public required string CalculateCurrency { get; set; }
    public required IList<Transaction> Transactions { get; set; }
}