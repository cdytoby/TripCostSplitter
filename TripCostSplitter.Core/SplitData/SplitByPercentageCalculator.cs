using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitByPercentageCalculator : ISplitCalculator
{
    public bool CanHandle(ISplitData splitData) => splitData is SplitByPercentage;

    public IList<RecipientInfo> CalculateDebit(PaymentData paymentData)
    {
        SplitByPercentage splitData = (SplitByPercentage)paymentData.SplitData!;
        Dictionary<string, decimal> portions = splitData.PersonPortionDict;

        if (!portions.Any() || !paymentData.PayerInfos.Any())
            return new List<RecipientInfo>();
        
        decimal totalPaid = paymentData.PayerInfos.Sum(p => p.Amount);
        List<RecipientInfo> result = [];
        foreach ((string participant, decimal portion) in portions)
        {
            result.Add(new RecipientInfo(participant, totalPaid * portion));
        }
        
        if (splitData.TotalExactValidation)
        {
            decimal resultSum = result.Sum(di => di.Amount);
            if (!resultSum.Equals(totalPaid))
            {
                RecipientInfo oldRecipientInfo = result[0];
                RecipientInfo newRecipientInfo =
                    oldRecipientInfo with
                    {
                        Amount = oldRecipientInfo.Amount + (totalPaid - resultSum)
                    };
                result[0] = newRecipientInfo;
            }
        }

        return result;
    }
}
