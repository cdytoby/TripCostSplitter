using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.Core.DataModels;

public class Payment: IPayment
{
    public required DateTime Date { get; set; }
    public string? Description { get; set; }
    public required IList<PaymentItem> PaymentItems { get; set; }
    public required IList<PayerInfo> PayerInfos { get; set; }
    public required IList<DebitInfo> DebitInfos { get; set; }
    public ISplitData? SplitData { get; set; }
    public decimal? ExchangeRateOverride { get; set; }
}