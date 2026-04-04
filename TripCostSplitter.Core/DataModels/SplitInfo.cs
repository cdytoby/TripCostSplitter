namespace TripCostSplitter.Core.DataModels;

public class SplitInfo
{
    public required Person Payee { get; set; }
    public required decimal Percentage { get; set; }
}