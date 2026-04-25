namespace TripCostSplitter.Core.DataModels;

public record DebtItem(string DebtorId, string CreditorId, decimal Amount);