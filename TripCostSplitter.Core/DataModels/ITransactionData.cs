using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace TripCostSplitter.Core.DataModels;

[JsonDerivedType(typeof(PaymentData), typeDiscriminator: "Payment")]
[JsonDerivedType(typeof(TransferData), typeDiscriminator: "Transfer")]
public interface ITransactionData
{
    DateTime Date { get; }
    
    string? Description { get; }
    
    string Currency { get; }
    
    decimal? ExchangeRateOverride { get; }
    
    ObservableCollection<PayerInfo> PayerInfos { get; }
}