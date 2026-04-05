namespace TripCostSplitter.Core.SplitData;

public class SplitEvenly : ISplitData
{
    public string SplitMethod => "Evenly";
    
    public bool TotalExactValidation { get; set; }
}
