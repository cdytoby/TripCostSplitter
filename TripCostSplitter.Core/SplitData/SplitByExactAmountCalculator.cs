using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitByExactAmountCalculator: ISplitCalculator
{
    public string SplitMethod => "ByExactAmount";
    
    public bool CanHandle(ISplitData splitData) => splitData is SplitByExactAmount;
    
    public IList<RecipientInfo> CalculateDebit(PaymentData paymentData)
    {
        SplitByExactAmount splitDataTyped = (SplitByExactAmount)paymentData.SplitData!;
        Dictionary<Person, decimal> amounts = splitDataTyped.PersonAmountDict;
        
        if (!amounts.Any() || !paymentData.PayerInfos.Any())
            return new List<RecipientInfo>();
        
        List<RecipientInfo> result = [];
        foreach ((Person participant, decimal exactAmount) in amounts)
        {
            result.Add(new RecipientInfo(participant, exactAmount));
        }
        
        return result;
    }
}