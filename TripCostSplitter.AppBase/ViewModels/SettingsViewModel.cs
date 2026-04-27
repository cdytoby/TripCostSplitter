using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.AppBase.ViewModels.CurrencyExchange;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.AppBase.ViewModels;

public partial class SettingsViewModel: ObservableRecipient
{
    [ObservableProperty]
    public partial CurrencyModel? DefaultCurrency { get; set; }
    
    [ObservableProperty]
    public partial ObservableCollection<ExchangeRateItemViewModel> ExchangeRateViewModels { get; private set; } = [];
    
    public IReadOnlyList<CurrencyModel> AvailableCurrencies { get; }
    
    private readonly INavigationService navigationService;
    private readonly IDataService dataService;
    private readonly IAppDispatcherService dispatcher;
    private readonly CurrencyService currencyService;
    
    private SettingsDataModel? settings;
    
    public SettingsViewModel(
        IDataService _dataService,
        IAppDispatcherService _dispatcher,
        CurrencyService _currencyService,
        INavigationService _navigationService): base(new WeakReferenceMessenger())
    {
        dataService = _dataService;
        dispatcher = _dispatcher;
        currencyService = _currencyService;
        navigationService = _navigationService;
        
        AvailableCurrencies = currencyService.GetAllCurrencyInfos();
        
        Messenger.Register<PropertyChangedMessage<CurrencyModel>>(this, MarkDuplicateExchangeRates);
        
        Task.Run(LoadSettingsAsync);
    }
    
    [RelayCommand]
    public async Task LoadSettingsAsync()
    {
        settings = await dataService.LoadSettingsAsync();
        
        dispatcher.Invoke(() =>
        {
            LoadDefaultCurrency();
            LoadLocalExchangeRate();
        });
    }
    
    private void LoadDefaultCurrency()
    {
        if (string.IsNullOrEmpty(settings!.DefaultCurrency))
        {
            DefaultCurrency = currencyService.GetCurrencyInfoFromCultureInfo(CultureInfo.CurrentCulture);
        }
        else
        {
            DefaultCurrency = AvailableCurrencies.Single(m => m.Code.Equals(settings.DefaultCurrency));
        }
    }
    
    private void LoadLocalExchangeRate()
    {
        ExchangeRateViewModels.Clear();
        foreach (CurrencyExchangeRateModel model in settings!.CachedExchangeRates)
        {
            ExchangeRateItemViewModel vm = new(Messenger);
            vm.Load(model, currencyService);
            ExchangeRateViewModels.Add(vm);
        }
    }
    
    [RelayCommand(CanExecute = nameof(CanSave))]
    public async Task SaveSettingsAsync()
    {
        settings!.DefaultCurrency = DefaultCurrency!.Code;
        List<CurrencyExchangeRateModel> result = [];
        foreach (ExchangeRateItemViewModel exchangeRateItemViewModel in ExchangeRateViewModels)
        {
            CurrencyExchangeRateModel? model = exchangeRateItemViewModel.Save();
            if (model != null)
                result.Add(model);
        }
        
        settings!.CachedExchangeRates = result;
        
        await dataService.SaveSettingsAsync(settings!);
        await navigationService.PopAsync();
    }
    
    [RelayCommand]
    public void AddExchangeRate()
    {
        ExchangeRateViewModels.Add(new ExchangeRateItemViewModel(Messenger)
        {
            LeftCurrency = DefaultCurrency
        });
    }
    
    [RelayCommand]
    public void RemoveExchangeRate(ExchangeRateItemViewModel rate)
    {
        ExchangeRateViewModels.Remove(rate);
    }
    
    private void MarkDuplicateExchangeRates(object recipient, PropertyChangedMessage<CurrencyModel> message)
    {
        HashSet<ExchangeRateItemViewModel> duplicates = [];
        for (int i = 0; i < ExchangeRateViewModels.Count - 1; i++)
        {
            for (int j = i + 1; j < ExchangeRateViewModels.Count; j++)
            {
                if (ExchangeRateViewModels[i].IsDuplicate(ExchangeRateViewModels[j]))
                {
                    duplicates.Add(ExchangeRateViewModels[i]);
                    duplicates.Add(ExchangeRateViewModels[j]);
                }
            }
        }
        
        foreach (ExchangeRateItemViewModel vm in ExchangeRateViewModels)
        {
            vm.Duplicate = duplicates.Contains(vm);
        }
    }
    
    private bool CanSave()
    {
        return settings != null;
    }
}