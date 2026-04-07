using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitByItemOwnershipCalculator : ISplitCalculator
{
    public bool CanHandle(ISplitData splitData) => splitData is SplitByItemOwnership;

    public IList<RecipientInfo> CalculateDebit(PaymentData paymentData)
    {
        SplitByItemOwnership splitDataTyped = (SplitByItemOwnership)paymentData.SplitData!;
        Dictionary<int, List<string>> ownershipDict = splitDataTyped.OwnershipGroups;
        IList<int> allParticipants = paymentData.ParticipantIds;

        if (!ownershipDict.Any() || !paymentData.PayerInfos.Any() || !allParticipants.Any())
            return new List<RecipientInfo>();

        List<RecipientInfo> result = [];
        
        
        foreach (int participant in allParticipants)
        {
            decimal personTotal = 0;
            if (ownershipDict.TryGetValue(participant, out List<string>? itemNames))
            {
                personTotal += itemNames.Sum(itemName => paymentData.PurchaseItems.First(pi => pi.Item.Equals(itemName)).Amount);
            }
            
            result.Add(new RecipientInfo(participant, personTotal));
            
        }
        
        return result;
    }
}
