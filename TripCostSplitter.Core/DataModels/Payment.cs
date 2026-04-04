namespace TripCostSplitter.Core.DataModels;

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