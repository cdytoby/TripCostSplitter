using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitByItemOwnership : ISplitData
{
    public string SplitMethod => "ByItemOwnership";
    public Dictionary<Person, IList<string>> OwnershipGroups { get; set; } = new();
}
