using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitByExactAmountCalculator: ISplitCalculator
{
    public string SplitMethod => "ByExactAmount";
    
    public bool CanHandle(ISplitData splitData) => splitData is SplitByExactAmount;
    
    public IList<DebitInfo> CalculateDebit(Payment payment)
    {
        SplitByExactAmount splitDataTyped = (SplitByExactAmount)payment.SplitData!;
        Dictionary<Person, decimal> amounts = splitDataTyped.PersonAmountDict;
        
        if (!amounts.Any() || !payment.PayerInfos.Any())
            return new List<DebitInfo>();
        
        List<DebitInfo> result = [];
        foreach ((Person participant, decimal exactAmount) in amounts)
        {
            result.Add(new DebitInfo
            {
                Payee = participant,
                Amount = exactAmount
            });
        }
        
        return result;
    }
}