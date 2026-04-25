using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitByPercentage: ISplitData
{
    public const string Key = "ByPercentage";

    public Dictionary<string, decimal> PersonPercentageDict { get; set; } = new();
    
    public bool TotalExactValidation { get; set; } = true;
}