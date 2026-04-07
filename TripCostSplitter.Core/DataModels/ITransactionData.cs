using System.Collections.ObjectModel;

namespace TripCostSplitter.Core.DataModels;

public interface ITransactionData
{
    string TransactionType { get; }
    
    DateTime Date { get; }
    
    string? Description { get; }
    
    string Currency { get; }
    
    decimal? ExchangeRateOverride { get; }
    
    ObservableCollection<PayerInfo> PayerInfos { get; }
}