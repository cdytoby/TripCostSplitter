using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitEvenlyCalculator : ISplitCalculator
{
    public string SplitMethod => "Evenly";

    public bool CanHandle(ISplitData splitData) => splitData is SplitEvenly;

    public IList<DebitInfo> CalculateDebit(Payment payment)
    {
        SplitEvenly splitData = (SplitEvenly)payment.SplitData!;
        IList<Person> allParticipants = payment.Participants;

        if (!allParticipants.Any() || !payment.PayerInfos.Any())
            return new List<DebitInfo>();

        decimal totalPaid = payment.PayerInfos.Sum(p => p.Amount);
        decimal perPerson = totalPaid / allParticipants.Count;

        List<DebitInfo> result = [];
        
        foreach (Person participant in allParticipants)
        {
            result.Add(new DebitInfo(participant, perPerson));
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
