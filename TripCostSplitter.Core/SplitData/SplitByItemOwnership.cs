using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitByItemOwnership : ISplitData
{
    public const string Key = "ByItemOwnership";

    /// <summary>
    /// Key is person id, value is the list of items the person owns
    /// </summary>
    public Dictionary<string, List<string>> OwnershipGroups { get; set; } = new();
    
    public void EnsurePerson(string personId)
    {
        if (!OwnershipGroups.TryGetValue(personId, out List<string>? _))
        {
            OwnershipGroups[personId] = [];
        }
    }
}
