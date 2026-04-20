namespace TripCostSplitter.Core.SplitData;

public class SplitEvenly : ISplitData
{
    public const string Key = "Evenly";
    
    public List<int> SplitParticipants { get; set; } = new();
    
    public bool TotalExactValidation { get; set; } = true;
}
