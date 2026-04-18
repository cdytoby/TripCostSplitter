using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.AppBase.ViewModels;

public partial class TransactionListViewModel: ObservableObject
{
    private readonly AccessManager accessManager;
    private readonly SessionService sessionService;
    private readonly INavigationService navigationService;
    
    public Travel Travel { get; }
    
    public TransactionListViewModel(
        AccessManager _accessManager,
        INavigationService _navigationService,
        SessionService _sessionService)
    {
        accessManager = _accessManager;
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
            Date = DateTime.Now,
            Currency = Travel.CalculateCurrency,
            PayerInfos = [],
            ParticipantIds = new(Travel.Participants.Select(p => p.Id)),
            PurchaseItems = []
        };
        Transaction transaction = new()
        {
            TransactionId = accessManager.GetNextId(),
            TransactionData = transactionData,
            RecipientInfos = []
        };
        Travel.Transactions.Add(transaction);
        sessionService.CurrentTransaction = transaction;
        await navigationService.PushAsync(ViewDefinition.TransactionDetailView);
    }
    
    [RelayCommand]
    public async Task EditTransaction(Transaction transaction)
    {
        sessionService.CurrentTransaction = transaction;
        await navigationService.PushAsync(ViewDefinition.TransactionDetailView);
    }
    
    [RelayCommand]
    public void DeleteTransaction(Transaction transaction)
    {
        Travel.Transactions.Remove(transaction);
        //todo update debts here
    }
}