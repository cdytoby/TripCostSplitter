using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.AppBase.ViewModels;

public partial class TransferDetailViewModel: ObservableObject
{
    [ObservableProperty]
    public partial CurrencyModel? Currency { get; set; }
    
    [ObservableProperty]
    public partial DateTime? Date { get; set; }
    
    [ObservableProperty]
    public partial TimeSpan? Time { get; set; }
    
    public IReadOnlyList<CurrencyModel> AvailableCurrencies { get; }
    public IReadOnlyList<Person> TravelParticipants { get; }
    public Transaction Transaction { get; }
    public TransferData? TransferData { get; }
    
    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand CancelCommand { get; }
    
    private readonly INavigationService navigationService;
    private readonly SessionService sessionService;
    
    private bool isLoaded;
    
    public TransferDetailViewModel(
        SessionService _sessionService,
        INavigationService _navigationService)
    {
        navigationService = _navigationService;
        sessionService = _sessionService;
        
        SaveCommand = new AsyncRelayCommand(Save);
        CancelCommand = new AsyncRelayCommand(Cancel);
        
        //todo exception or load state with nullable
        Transaction = sessionService.CurrentTransaction!.Copy();
        TravelParticipants = sessionService.CurrentTravel!.Participants.ToList();
        AvailableCurrencies =
        [
            ..CurrencyService.GetCurrencyInfos(
                [sessionService.CurrentTravel.CalculateCurrency, ..sessionService.CurrentTravel.AdditionalCurrencies])
        ];
        
        TransferData = Transaction.TransactionData as TransferData;
        
        Date = new DateTime(Transaction.Date.Year, Transaction.Date.Month, Transaction.Date.Day);
        Time = new TimeSpan(Transaction.Date.Hour, Transaction.Date.Minute, Transaction.Date.Second);
        Currency = CurrencyService.GetCurrencyInfo(Transaction.Currency);
        
        isLoaded = true;
    }
    
    partial void OnCurrencyChanged(CurrencyModel? oldValue, CurrencyModel? newValue)
    {
        if (!isLoaded || Currency == null)
            return;
        Transaction.Currency = Currency.Code;
    }
    
    partial void OnDateChanged(DateTime? oldValue, DateTime? newValue)
    {
        OnDateTimeChanged();
    }
    
    partial void OnTimeChanged(TimeSpan? oldValue, TimeSpan? newValue)
    {
        OnDateTimeChanged();
    }
    
    private void OnDateTimeChanged()
    {
        if (!isLoaded || Date == null || Time == null)
            return;
        Transaction.Date = new DateTime(
            Date.Value.Year, Date.Value.Month, Date.Value.Day, Time.Value.Hours, Time.Value.Minutes, Time.Value.Seconds,
            DateTimeKind.Unspecified);
    }
    
    private async Task Save()
    {
        CalculateRecipient();
        
        await sessionService.SaveTransaction(Transaction);
        
        await navigationService.PopAsync();
    }
    
    private void CalculateRecipient()
    {
        if (TransferData == null)
            return;
        
        Transaction.RecipientInfos = new List<RecipientInfo>
        {
            new(TransferData.ToPersonId, TransferData.Amount)
        };
    }
    
    private async Task Cancel()
    {
        await navigationService.PopAsync();
    }
}
