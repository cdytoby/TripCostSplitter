using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitByExactAmount: ISplitData
{
    public const string Key = "ByExactAmount";
    
    public Dictionary<int, decimal> PersonIdAmountDict { get; set; } = new();
}