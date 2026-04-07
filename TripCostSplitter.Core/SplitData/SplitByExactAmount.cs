using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitByExactAmount: ISplitData
{
    public Dictionary<int, decimal> PersonIdAmountDict { get; set; } = new();
}