namespace TripCostSplitter.Core.SplitData;

public class SplitEvenly : ISplitData
{
    public const string Key = "Evenly";

    public bool TotalExactValidation { get; set; }
}
