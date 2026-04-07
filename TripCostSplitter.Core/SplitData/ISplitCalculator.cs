using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public interface ISplitCalculator
{
    bool CanHandle(ISplitData splitData);
    
    IList<RecipientInfo> CalculateDebit(PaymentData paymentData);
}