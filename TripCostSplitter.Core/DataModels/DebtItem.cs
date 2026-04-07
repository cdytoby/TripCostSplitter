namespace TripCostSplitter.Core.DataModels;

public record DebtItem(int DebtorId, int CreditorId, decimal Amount);