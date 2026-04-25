using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core;

public class DebtCalculator
{
    public IEnumerable<DebtItem> CalculateDebts(Travel travel)
    {
        // Calculate net balance for each person
        Dictionary<string, decimal> balances = new();

        foreach (Transaction transactions in travel.Transactions)
        {
            ITransactionData transactionData = transactions.TransactionData;
            // Add amounts paid by payers
            foreach (PayerInfo payerInfo in transactionData.PayerInfos)
            {
                balances.TryAdd(payerInfo.PayerId, 0);
                balances[payerInfo.PayerId] += payerInfo.Amount;
            }

            // Subtract amounts owed by debtors
            foreach (RecipientInfo debitInfo in transactions.RecipientInfos)
            {
                balances.TryAdd(debitInfo.RecipientId, 0);
                balances[debitInfo.RecipientId] -= debitInfo.Amount;
            }
        }

        // Separate creditors (positive balance) and debtors (negative balance)
        List<KeyValuePair<string, decimal>> creditors = balances.Where(b => b.Value > 0).OrderByDescending(b => b.Value).ToList();
        List<KeyValuePair<string, decimal>> debtors = balances.Where(b => b.Value < 0).OrderBy(b => b.Value).ToList();

        List<DebtItem> debts = [];

        int creditorIndex = 0;
        int debtorIndex = 0;

        // Match debtors with creditors
        while (creditorIndex < creditors.Count && debtorIndex < debtors.Count)
        {
            KeyValuePair<string, decimal> creditor = creditors[creditorIndex];
            KeyValuePair<string, decimal> debtor = debtors[debtorIndex];

            decimal amountToSettle = Math.Min(creditor.Value, Math.Abs(debtor.Value));

            debts.Add(new DebtItem(debtor.Key, creditor.Key, amountToSettle));

            creditors[creditorIndex] = new KeyValuePair<string, decimal>(creditor.Key, creditor.Value - amountToSettle);
            debtors[debtorIndex] = new KeyValuePair<string, decimal>(debtor.Key, debtor.Value + amountToSettle);

            if (creditors[creditorIndex].Value == 0)
                creditorIndex++;
            if (debtors[debtorIndex].Value == 0)
                debtorIndex++;
        }

        return debts;
    }
}