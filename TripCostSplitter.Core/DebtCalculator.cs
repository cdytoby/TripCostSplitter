using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Core;

public class DebtCalculator
{
    public IEnumerable<DebtItem> CalculateDebts(Travel travel)
    {
        // Calculate net balance for each person
        Dictionary<Person, decimal> balances = new();

        foreach (IPayment p in travel.Payments)
        {
            Payment payment = (Payment)p;
            // Add amounts paid by payers
            foreach (PayerInfo payerInfo in payment.PayerInfos)
            {
                balances.TryAdd(payerInfo.Payer, 0);
                balances[payerInfo.Payer] += payerInfo.Amount;
            }

            // Subtract amounts owed by debtors
            foreach (DebitInfo debitInfo in payment.DebitInfos)
            {
                balances.TryAdd(debitInfo.Payee, 0);
                balances[debitInfo.Payee] -= debitInfo.Amount;
            }
        }

        // Separate creditors (positive balance) and debtors (negative balance)
        List<KeyValuePair<Person, decimal>> creditors = balances.Where(b => b.Value > 0).OrderByDescending(b => b.Value).ToList();
        List<KeyValuePair<Person, decimal>> debtors = balances.Where(b => b.Value < 0).OrderBy(b => b.Value).ToList();

        List<DebtItem> debts = [];

        int creditorIndex = 0;
        int debtorIndex = 0;

        // Match debtors with creditors
        while (creditorIndex < creditors.Count && debtorIndex < debtors.Count)
        {
            KeyValuePair<Person, decimal> creditor = creditors[creditorIndex];
            KeyValuePair<Person, decimal> debtor = debtors[debtorIndex];

            decimal amountToSettle = Math.Min(creditor.Value, Math.Abs(debtor.Value));

            debts.Add(new DebtItem
            {
                Debtor = debtor.Key,
                Creditor = creditor.Key,
                Amount = amountToSettle
            });

            creditors[creditorIndex] = new KeyValuePair<Person, decimal>(creditor.Key, creditor.Value - amountToSettle);
            debtors[debtorIndex] = new KeyValuePair<Person, decimal>(debtor.Key, debtor.Value + amountToSettle);

            if (creditors[creditorIndex].Value == 0)
                creditorIndex++;
            if (debtors[debtorIndex].Value == 0)
                debtorIndex++;
        }

        return debts;
    }
}