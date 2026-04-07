using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitEvenlyCalculator : ISplitCalculator
{
    public bool CanHandle(ISplitData splitData) => splitData is SplitEvenly;

    public IList<RecipientInfo> CalculateDebit(PaymentData paymentData)
    {
        SplitEvenly splitData = (SplitEvenly)paymentData.SplitData!;
        IList<int> allParticipants = paymentData.ParticipantIds;

        if (!allParticipants.Any() || !paymentData.PayerInfos.Any())
            return new List<RecipientInfo>();

        decimal totalPaid = paymentData.PayerInfos.Sum(p => p.Amount);
        decimal perPerson = totalPaid / allParticipants.Count;

        List<RecipientInfo> result = [];
        
        foreach (int participant in allParticipants)
        {
            result.Add(new RecipientInfo(participant, perPerson));
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
