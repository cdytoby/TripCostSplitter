using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitByPercentageCalculator : ISplitCalculator
{
    public string SplitMethod => "ByPercentage";

    public bool CanHandle(ISplitData splitData) => splitData is SplitByPercentage;

    public IList<RecipientInfo> CalculateDebit(PaymentData paymentData)
    {
        SplitByPercentage splitData = (SplitByPercentage)paymentData.SplitData!;
        Dictionary<Person, decimal> percentages = splitData.PersonPercentageDict;

        if (!percentages.Any() || !paymentData.PayerInfos.Any())
            return new List<RecipientInfo>();
        
        decimal totalPaid = paymentData.PayerInfos.Sum(p => p.Amount);
        List<RecipientInfo> result = [];
        foreach ((Person participant, decimal percentage) in percentages)
        {
            result.Add(new RecipientInfo(participant, totalPaid * (percentage / 100)));
        }
        
        if (splitData.TotalExactValidation)
        {
            decimal resultSum = result.Sum(di => di.Amount);
            if (!resultSum.Equals(totalPaid))
            {
                RecipientInfo oldRecipientInfo = result[0];
                RecipientInfo newRecipientInfo =
                    new RecipientInfo(oldRecipientInfo.Recipient, oldRecipientInfo.Amount + (totalPaid - resultSum));
                result[0] = newRecipientInfo;
            }
        }

        return result;
    }
}
