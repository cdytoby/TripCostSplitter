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
            result.Add(new DebitInfo
            {
                Payee = participant,
                Amount = perPerson
            });
        }
        
        if (splitData.TotalExactValidation)
        {
            decimal resultSum = result.Sum(di => di.Amount);
            if (!resultSum.Equals(totalPaid))
            {
                result.Last().Amount += totalPaid - resultSum;
            }
        }

        return result;
    }
}
