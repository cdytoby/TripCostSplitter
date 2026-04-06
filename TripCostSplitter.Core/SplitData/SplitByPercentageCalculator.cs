using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitByPercentageCalculator : ISplitCalculator
{
    public string SplitMethod => "ByPercentage";

    public bool CanHandle(ISplitData splitData) => splitData is SplitByPercentage;

    public IList<DebitInfo> CalculateDebit(Payment payment)
    {
        SplitByPercentage splitData = (SplitByPercentage)payment.SplitData!;
        Dictionary<Person, decimal> percentages = splitData.PersonPercentageDict;

        if (!percentages.Any() || !payment.PayerInfos.Any())
            return new List<DebitInfo>();
        
        decimal totalPaid = payment.PayerInfos.Sum(p => p.Amount);
        List<DebitInfo> result = [];
        foreach ((Person participant, decimal percentage) in percentages)
        {
            result.Add(new DebitInfo(participant, totalPaid * (percentage / 100)));
        }
        
        if (splitData.TotalExactValidation)
        {
            decimal resultSum = result.Sum(di => di.Amount);
            if (!resultSum.Equals(totalPaid))
            {
                DebitInfo oldDebitInfo = result[0];
                DebitInfo newDebitInfo =
                    new DebitInfo(oldDebitInfo.Recipient, oldDebitInfo.Amount + (totalPaid - resultSum));
                result[0] = newDebitInfo;
            }
        }

        return result;
    }
}
