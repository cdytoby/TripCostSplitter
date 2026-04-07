using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitByPercentage: ISplitData
{
    public Dictionary<int, decimal> PersonPercentageDict { get; set; } = new();
    
    public bool TotalExactValidation { get; set; }
}