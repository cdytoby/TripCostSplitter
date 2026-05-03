using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.AppBase.ViewModels;

public partial class TransactionListViewModel: ObservableObject
{
    public Travel Travel { get; }
    
    private readonly SessionService sessionService;
    private readonly INavigationService navigationService;
    
    public TransactionListViewModel(
        INavigationService _navigationService,
        SessionService _sessionService)
    {
        navigationService = _navigationService;
        sessionService = _sessionService;
        
        //todo exception or load state with nullable
        Travel = sessionService.CurrentTravel!;
    }
    
    [RelayCommand]
    public async Task AddTransaction()
    {
        PaymentData transactionData = new()
        {
            PayerInfos = [],
            ParticipantIds = new(Travel.Participants.Select(p => p.Id)),
            PurchaseItems = []
        };
        Transaction transaction = new()
        {
            Date = DateTime.Now,
            DateTimeZone = TimeZoneInfo.Local,
            TransactionId = AccessManager.GetNewId(),
            Currency = Travel.CalculateCurrency,
            TransactionData = transactionData
        };
        Travel.Transactions.Add(transaction);
        sessionService.CurrentTransaction = transaction;
        await navigationService.PushAsync(ViewDefinition.PaymentDetailView);
    }
    
    [RelayCommand]
    public async Task EditTransaction(Transaction transaction)
    {
        sessionService.CurrentTransaction = transaction;
        await navigationService.PushAsync(ViewDefinition.PaymentDetailView);
    }
    
    [RelayCommand]
    public async Task DeleteTransaction(Transaction transaction)
    {
        Travel.Transactions.Remove(transaction);
        await sessionService.Save();
        //todo update debts here
    }
}