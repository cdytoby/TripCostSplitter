namespace TripCostSplitter.Core.DataModels;

public record DebtItem(Person Debtor, Person Creditor, decimal Amount);