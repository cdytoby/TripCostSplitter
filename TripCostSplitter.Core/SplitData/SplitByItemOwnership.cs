using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitByItemOwnership : ISplitData
{
    public const string Key = "ByItemOwnership";

    public Dictionary<int, List<string>?> OwnershipGroups { get; set; } = new();
}
