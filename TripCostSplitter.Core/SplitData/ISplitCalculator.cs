using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public interface ISplitCalculator
{
    string SplitMethod { get; }
    
    bool CanHandle(ISplitData splitData);
    
    IList<DebitInfo> CalculateDebit(Payment payment);
}