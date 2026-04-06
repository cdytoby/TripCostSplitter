using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core.SplitData;

public class SplitByItemOwnershipCalculator : ISplitCalculator
{
    public string SplitMethod => "ByItemOwnership";

    public bool CanHandle(ISplitData splitData) => splitData is SplitByItemOwnership;

    public IList<DebitInfo> CalculateDebit(Payment payment)
    {
        SplitByItemOwnership splitDataTyped = (SplitByItemOwnership)payment.SplitData!;
        Dictionary<Person, IList<string>> ownershipDict = splitDataTyped.OwnershipGroups;
        IList<Person> allParticipants = payment.Participants;

        if (!ownershipDict.Any() || !payment.PayerInfos.Any() || !allParticipants.Any())
            return new List<DebitInfo>();

        List<DebitInfo> result = [];
        
        
        foreach (Person participant in allParticipants)
        {
            decimal personTotal = 0;
            if (ownershipDict.TryGetValue(participant, out IList<string>? itemNames))
            {
                personTotal += itemNames.Sum(itemName => payment.PurchaseItems.First(pi => pi.Item.Equals(itemName)).Amount);
            }
            
            result.Add(new DebitInfo(participant, personTotal));
            
        }
        
        return result;
    }
}
