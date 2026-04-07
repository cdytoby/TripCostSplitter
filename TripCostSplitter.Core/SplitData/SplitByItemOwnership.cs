using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitByItemOwnership : ISplitData
{
    public Dictionary<int, List<string>> OwnershipGroups { get; set; } = new();
}
