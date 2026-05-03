using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.AppBase.ViewModels.SplitViewModels;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.AppBase.ViewModels;

public partial class PaymentDetailViewModel: ObservableObject
{
    public IReadOnlyList<CurrencyModel> AvailableCurrencies { get; }
    public IReadOnlyList<Person> TravelParticipants { get; }
    
    public List<string> AvailableSplitMethods { get; } =
        [SplitByExactAmount.Key, SplitByPercentage.Key, SplitByItemOwnership.Key, SplitEvenly.Key];
    
    [ObservableProperty]
    public partial DateTime? Date { get; set; }
    
    [ObservableProperty]
    public partial TimeSpan? Time { get; set; }
    
    [ObservableProperty]
    public partial SplitDataViewModelBase? SplitDataViewModel { get; set; }
    
    [ObservableProperty]
    public partial string? CurrentSplitMethod { get; set; }
    
    public Transaction Transaction { get; }
    public PaymentData? PaymentData { get; }
    
    private readonly List<ISplitCalculator> splitCalculators;
    private readonly INavigationService navigationService;
    private readonly SessionService sessionService;
    private readonly SplitDataViewModelService splitDataViewModelService;
    
    private bool isLoaded;
    
    public PaymentDetailViewModel(
        SessionService _sessionService,
        IEnumerable<ISplitCalculator> _splitCalculators,
        INavigationService _navigationService,
        CurrencyService _currencyService,
        SplitDataViewModelService _splitDataViewModelService)
    {
        splitCalculators = _splitCalculators.ToList();
        navigationService = _navigationService;
        splitDataViewModelService = _splitDataViewModelService;
        
        //todo exception or load state with nullable
        sessionService = _sessionService;
        Transaction = _sessionService.CurrentTransaction!;
        TravelParticipants = _sessionService.CurrentTravel!.Participants.ToList();
        AvailableCurrencies =
        [
            .._currencyService.GetCurrencyInfos(
                [_sessionService.CurrentTravel.CalculateCurrency, .._sessionService.CurrentTravel.AdditionalCurrencies])
        ];
        
        //todo exception or load state with nullable
        PaymentData = Transaction.TransactionData as PaymentData;
        if (PaymentData != null && PaymentData.PurchaseItems.Count == 0)
        {
            PaymentData.PurchaseItems.Add(new PurchaseItem("total cost", 0));
        }
        
        Date = new DateTime(Transaction.Date.Year, Transaction.Date.Month, Transaction.Date.Day);
        Time = new TimeSpan(Transaction.Date.Hour, Transaction.Date.Minute, Transaction.Date.Second);
        
        LoadSplitData();
        
        isLoaded = true;
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
    
    [RelayCommand]
    public void AddPayer(Person person)
    {
        PaymentData?.PayerInfos.Add(new PayerInfo(person.Id, 0));
    }
    
    [RelayCommand]
    public void RemovePayer(PayerInfo item)
    {
        PaymentData?.PayerInfos.Remove(item);
    }
    
    [RelayCommand]
    public void AddPurchaseItem()
    {
        PaymentData?.PurchaseItems.Add(new PurchaseItem("", 0));
    }
    
    [RelayCommand]
    public void RemovePurchaseItem(PurchaseItem item)
    {
        PaymentData?.PurchaseItems.Remove(item);
    }
    
    private void LoadSplitData()
    {
        if (PaymentData == null)
            return;
        (CurrentSplitMethod, SplitDataViewModel) =
            splitDataViewModelService.LoadSplitDataViewModel(TravelParticipants, PaymentData);
    }
    
    partial void OnCurrentSplitMethodChanged(string? value)
    {
        if (PaymentData == null)
            return;
        SplitDataViewModel = splitDataViewModelService.LoadSplitDataViewModel(
            CurrentSplitMethod, TravelParticipants, PaymentData);
    }
    
    [RelayCommand]
    public async Task Save()
    {
        ApplyPaymentData();
        
        CalculateRecipient();
        
        await sessionService.Save();
        
        await navigationService.PopAsync();
    }
    
    private void ApplyPaymentData()
    {
        if (PaymentData != null && CurrentSplitMethod != null && SplitDataViewModel != null)
        {
            PaymentData.SplitData = SplitDataViewModel.Save();
        }
    }
    
    private void CalculateRecipient()
    {
        if (PaymentData?.SplitData == null)
            return;
        
        ISplitCalculator? calculator = splitCalculators.FirstOrDefault(c => c.CanHandle(PaymentData.SplitData));
        
        if (calculator != null)
        {
            IList<RecipientInfo> debits = calculator.CalculateDebit(PaymentData);
            Transaction.RecipientInfos = new List<RecipientInfo>(debits);
        }
    }
    
    [RelayCommand]
    public async Task Cancel()
    {
        await navigationService.PopAsync();
    }
}