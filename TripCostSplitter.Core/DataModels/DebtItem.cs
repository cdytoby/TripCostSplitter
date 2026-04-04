namespace TripCostSplitter.Core.DataModels;

public class DebtItem
{
    public required Person Debtor { get; set; }
    public required Person Creditor { get; set; }
    public decimal Amount { get; set; }
}