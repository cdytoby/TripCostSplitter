using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitByExactAmountCalculator: ISplitCalculator
{
    public bool CanHandle(ISplitData splitData) => splitData is SplitByExactAmount;
    
    public IList<RecipientInfo> CalculateDebit(PaymentData paymentData)
    {
        SplitByExactAmount splitDataTyped = (SplitByExactAmount)paymentData.SplitData!;
        Dictionary<int, decimal> amounts = splitDataTyped.PersonIdAmountDict;
        
        if (!amounts.Any() || !paymentData.PayerInfos.Any())
            return new List<RecipientInfo>();
        
        List<RecipientInfo> result = [];
        foreach ((int participant, decimal exactAmount) in amounts)
        {
            result.Add(new RecipientInfo(participant, exactAmount));
        }
        
        return result;
    }
}