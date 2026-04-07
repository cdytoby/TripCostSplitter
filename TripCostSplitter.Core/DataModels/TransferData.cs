using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TripCostSplitter.Core.DataModels;

public partial class TransferData : ObservableObject, ITransactionData
{
    [ObservableProperty]
    public required partial DateTime Date { get; set; }

    [ObservableProperty]
    public partial string? Description { get; set; }

    [ObservableProperty]
    public required partial string Currency { get; set; }

    [ObservableProperty]
    public partial decimal? ExchangeRateOverride { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PayerInfos))]
    public required partial int FromPersonId { get; set; }

    [ObservableProperty]
    public required partial int ToPersonId { get; set; }

    [ObservableProperty]
    public required partial decimal Amount { get; set; }

    public ObservableCollection<PayerInfo> PayerInfos => [new(FromPersonId, Amount)];
}
