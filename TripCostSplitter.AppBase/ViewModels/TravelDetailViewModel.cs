using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.AppBase.ViewModels;

public partial class TravelDetailViewModel: ObservableObject
{
    public Travel Travel { get; }
    public CurrencyModel[] AllCurrencies { get; }
    
    [ObservableProperty]
    public partial bool IsCurrencyExchangeMissing { get; set; }
    
    public IRelayCommand AddAdditionalCurrencyCommand { get; }
    public IRelayCommand DeleteAdditionalCurrencyCommand { get; }
    public IRelayCommand AddPersonCommand { get; }
    public IRelayCommand DeletePersonCommand { get; }
    
    private readonly SessionService sessionService;
    private readonly CurrencyService currencyService;
    
    public TravelDetailViewModel(
        SessionService _sessionService,
        CurrencyService _currencyService)
    {
        sessionService = _sessionService;
        currencyService = _currencyService;
        AllCurrencies = CurrencyService.GetAllCurrencyInfos();
        
        AddAdditionalCurrencyCommand = new AsyncRelayCommand<string>(AddAdditionalCurrency);
        DeleteAdditionalCurrencyCommand = new AsyncRelayCommand<string>(DeleteAdditionalCurrency);
        AddPersonCommand = new AsyncRelayCommand(AddPerson);
        DeletePersonCommand = new AsyncRelayCommand<Person>(DeletePerson);
        
        //todo exception or load state with nullable
        Travel = sessionService.CurrentTravel!;
        
        CheckExchangeAvailable();
    }
    
    private async Task AddAdditionalCurrency(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return;
        if (Travel.CalculateCurrency == code)
            return;
        if (Travel.AdditionalCurrencies.Contains(code))
            return;
        
        Travel.AdditionalCurrencies.Add(code);
        
        CheckExchangeAvailable();
        
        await Save();
    }
    
    private void CheckExchangeAvailable()
    {
        if (Travel.AdditionalCurrencies.Any(currency =>
            currencyService.GetExchangeRate(Travel.CalculateCurrency, currency) == 0))
        {
            IsCurrencyExchangeMissing = true;
            return;
        }
        
        IsCurrencyExchangeMissing = false;
    }
    
    private async Task DeleteAdditionalCurrency(string? code)
    {
        //todo delete only when currency is not used
        if (code == null)
            return;
        
        Travel.AdditionalCurrencies.Remove(code);
        
        CheckExchangeAvailable();
        
        await Save();
    }
    
    private async Task AddPerson()
    {
        string newId = AccessManager.GetNewId();
        Travel.Participants.Add(new Person(newId, "Traveller " + newId[..3]));
        await Save();
    }
    
    //todo delete person when a person doesn't involve any transactions, and mark button if can't delete
    private async Task DeletePerson(Person? deletePerson)
    {
        if (deletePerson == null)
            return;
        
        bool canDelete = Travel.Transactions.All(transaction =>
            !transaction.RecipientInfos.Select(t => t.RecipientId).ToHashSet().Contains(deletePerson.Id));
        
        if (canDelete)
        {
            Travel.Participants.Remove(deletePerson);
            await Save();
        }
    }
    
    public async Task Save()
    {
        await sessionService.SaveTravel();
    }
}