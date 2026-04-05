using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitByExactAmount: ISplitData
{
    public string SplitMethod => "ByExactAmount";
    
    public Dictionary<Person, decimal> PersonAmountDict { get; set; } = new();
}