namespace TripCostSplitter.Core;

public class Payment: IPayment
{
    public required DateTime Date { get; set; }
    public string? Description { get; set; }
    public required IEnumerable<PaymentItem> PaymentItems { get; set; }
    public required IEnumerable<PayerInfo> PayerInfos { get; set; }
    public SplitMethod SplitMethod { get; set; } = SplitMethod.Custom;
    public IEnumerable<SplitInfo>? SplitInfos { get; set; }
    public required IEnumerable<DebitInfo> DebitInfos { get; set; }
    public decimal? ExchangeRateOverride { get; set; }
}

public class PaymentItem
{
    public string? Item { get; set; }
    public required decimal Amount { get; set; }
    public required string Currency { get; set; }
}

public class PayerInfo
{
    public required Person Payer { get; set; }
    public required decimal Amount { get; set; }
    public required string Currency { get; set; }
}

public class SplitInfo
{
    public required Person Payee { get; set; }
    public required decimal Percentage { get; set; }
}

public class DebitInfo
{
    public required Person Payee { get; set; }
    public required decimal Amount { get; set; }
    public required string Currency { get; set; }
}