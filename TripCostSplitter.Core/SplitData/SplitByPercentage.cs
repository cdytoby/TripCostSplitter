using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitByPercentage: ISplitData
{
    public string SplitMethod => "ByPercentage";
    
    public Dictionary<Person, decimal> PersonPercentageDict { get; set; } = new();
    
    public bool TotalExactValidation { get; set; }
}