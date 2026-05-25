using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TripCostSplitter.AppBase.Messages;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.AppBase.ViewModels.SplitViewModels;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;
using TripCostSplitter.Core.SplitData;

namespace TripCostSplitter.AppBase.ViewModels;

public partial class PaymentDetailViewModel: ObservableRecipient
{
    [ObservableProperty]
    [NotifyPropertyChangedRecipients]
    public partial CurrencyModel? Currency { get; set; }
    
    [ObservableProperty]
    public partial DateTime? Date { get; set; }
    
    [ObservableProperty]
    public partial TimeSpan? Time { get; set; }
    
    [ObservableProperty]
    public partial bool EnableItemList { get; set; }
    
    [ObservableProperty]
    public partial bool HasPurchaseItemValidationError { get; set; } = false;
    
    [ObservableProperty]
    public partial SplitDataViewModelBase? SplitDataViewModel { get; set; }
    
    [ObservableProperty]
    public partial string? CurrentSplitMethod { get; set; }
    
    [ObservableProperty]
    public partial bool HasSplitDataValidationError { get; set; } = false;
    
    public List<string> AvailableSplitMethods { get; } = SplitDataViewModelService.GetAvaliableSplitMethods();
    public IReadOnlyList<CurrencyModel> AvailableCurrencies { get; }
    public IReadOnlyList<Person> TravelParticipants { get; }
    public Transaction Transaction { get; }
    public PaymentData? PaymentData { get; }
    
    public IRelayCommand<Person> AddPayerCommand { get; }
    public IRelayCommand<PayerInfo> RemovePayerCommand { get; }
    public IRelayCommand AddPurchaseItemCommand { get; }
    public IRelayCommand<PurchaseItem> DuplicatePurchaseItemCommand { get; }
    public IRelayCommand<PurchaseItem> RemovePurchaseItemCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand CancelCommand { get; }
    
    private readonly List<ISplitCalculator> splitCalculators;
    private readonly INavigationService navigationService;
    private readonly SessionService sessionService;
    private readonly CurrencyService currencyService;
    private readonly SplitDataViewModelService splitDataViewModelService;
    
    private bool isLoaded;
    private IList<PurchaseItem>? cachedPurchaseItems;
    
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
        sessionService = _sessionService;
        currencyService = _currencyService;
        
        AddPayerCommand = new RelayCommand<Person>(AddPayer!);
        RemovePayerCommand = new RelayCommand<PayerInfo>(RemovePayer!);
        AddPurchaseItemCommand = new RelayCommand(AddPurchaseItem);
        DuplicatePurchaseItemCommand = new RelayCommand<PurchaseItem>(DuplicatePurchaseItem!);
        RemovePurchaseItemCommand = new RelayCommand<PurchaseItem>(RemovePurchaseItem!);
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
        
        //todo exception or load state with nullable
        PaymentData = Transaction.TransactionData as PaymentData;
        
        Date = new DateTime(Transaction.Date.Year, Transaction.Date.Month, Transaction.Date.Day);
        Time = new TimeSpan(Transaction.Date.Hour, Transaction.Date.Minute, Transaction.Date.Second);
        Currency = CurrencyService.GetCurrencyInfo(Transaction.Currency);
        
        LoadPurchasedItems();
        LoadSplitData();
        
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
    
    partial void OnEnableItemListChanged(bool value)
    {
        if (PaymentData == null)
            return;
        if (value)
        {
            if (cachedPurchaseItems == null || cachedPurchaseItems.Count == 0)
                return;
            PaymentData.PurchaseItems.Clear();
            foreach (PurchaseItem item in cachedPurchaseItems)
            {
                PaymentData.PurchaseItems.Add(item);
            }
        }
        else
        {
            cachedPurchaseItems = new List<PurchaseItem>(PaymentData.PurchaseItems);
            PaymentData.PurchaseItems.Clear();
            PaymentData.PurchaseItems.Add(new PurchaseItem("Total cost", GetTotalPayment()));
        }
        
        UpdatePurchaseItemValidation();
    }
    
    private decimal GetTotalPayment()
    {
        return PaymentData!.PayerInfos.Sum(i => i.Amount);
    }
    
    private decimal GetTotalPurchasedItems()
    {
        return PaymentData!.PurchaseItems.Sum(i => i.Price);
    }
    
    private void AddPayer(Person person)
    {
        if (PaymentData?.PayerInfos.Any(p => p.PayerId.Equals(person.Id)) ?? false)
        {
            return;
        }
        
        PaymentData?.PayerInfos.Add(new PayerInfo(person.Id));
    }
    
    private void RemovePayer(PayerInfo item)
    {
        PaymentData?.PayerInfos.Remove(item);
        UpdatePurchaseItemValidation();
    }
    
    private void LoadPurchasedItems()
    {
        if (PaymentData == null)
            return;
        int count = PaymentData.PurchaseItems.Count;
        switch (count)
        {
            case 0:
                PaymentData.PurchaseItems.Add(new PurchaseItem("Total cost", GetTotalPayment()));
                EnableItemList = false;
                break;
            case 1 when
                PaymentData.PurchaseItems.Single().Price.Equals(GetTotalPayment()):
                EnableItemList = false;
                break;
            default:
                EnableItemList = true;
                break;
        }
        
        UpdatePurchaseItemValidation();
    }
    
    public void PaymentPriceUpdated()
    {
        UpdatePurchaseItemValidation();
        Messenger.Send(new PaymentTotalValueChangedMessage(GetTotalPayment()));
        Messenger.Send(new PaymentItemsChangedMessage());
    }
    
    private void AddPurchaseItem()
    {
        PaymentData?.PurchaseItems.Add(new PurchaseItem("", 0));
        Messenger.Send(new PaymentItemsChangedMessage());
    }
    
    private void DuplicatePurchaseItem(PurchaseItem item)
    {
        int index = PaymentData?.PurchaseItems.IndexOf(item) ?? 0;
        PaymentData?.PurchaseItems.Insert(index, new PurchaseItem(item.ItemName, item.Price));
        UpdatePurchaseItemValidation();
        Messenger.Send(new PaymentItemsChangedMessage());
    }
    
    private void RemovePurchaseItem(PurchaseItem item)
    {
        if (PaymentData?.PurchaseItems.Count <= 1)
            return;
        PaymentData?.PurchaseItems.Remove(item);
        UpdatePurchaseItemValidation();
        Messenger.Send(new PaymentItemsChangedMessage());
    }
    
    public void PurchaseItemUpdated()
    {
        UpdatePurchaseItemValidation();
        Messenger.Send(new PaymentItemsChangedMessage());
    }
    
    private void UpdatePurchaseItemValidation()
    {
        if (PaymentData == null || !isLoaded)
            return;
        if (!EnableItemList)
        {
            PaymentData.PurchaseItems.Single().Price = GetTotalPayment();
        }
        
        AdjustPurchaseItemNames();
        HasPurchaseItemValidationError = GetTotalPurchasedItems() != GetTotalPayment();
    }
    
    private void AdjustPurchaseItemNames()
    {
        if (PaymentData == null || !isLoaded)
            return;
        
        HashSet<string> uniqueItemNames = [];
        foreach (PurchaseItem item in PaymentData.PurchaseItems)
        {
            if (uniqueItemNames.Add(item.ItemName))
                continue;
            do
            {
                item.ItemName = $"{item.ItemName} S";
            } while (uniqueItemNames.Contains(item.ItemName));
            
            uniqueItemNames.Add(item.ItemName);
        }
    }
    
    private void LoadSplitData()
    {
        if (PaymentData == null || Currency == null)
            return;
        (CurrentSplitMethod, SplitDataViewModel) =
            splitDataViewModelService.LoadSplitDataViewModel(PaymentData, TravelParticipants, Currency, Messenger);
    }
    
    partial void OnCurrentSplitMethodChanged(string? value)
    {
        if (PaymentData == null || Currency == null)
            return;
        SplitDataViewModel = splitDataViewModelService.LoadSplitDataViewModel(
            CurrentSplitMethod, PaymentData, TravelParticipants, Currency, Messenger);
    }
    
    private async Task Save()
    {
        ApplyPaymentData();
        
        CalculateRecipient();
        
        await sessionService.SaveTransaction(Transaction);
        
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
    
    private async Task Cancel()
    {
        await navigationService.PopAsync();
    }
}