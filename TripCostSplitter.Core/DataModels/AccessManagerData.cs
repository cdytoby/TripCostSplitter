namespace TripCostSplitter.Core.DataModels;

public class AccessManagerData
{
    public int NextId { get; set; }
    
    public AccessManagerData Clone()
    {
        return (AccessManagerData)MemberwiseClone();
    }
}