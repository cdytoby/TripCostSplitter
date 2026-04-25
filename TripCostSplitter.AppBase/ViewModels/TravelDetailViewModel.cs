using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.Core;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.AppBase.ViewModels;

public partial class TravelDetailViewModel: ObservableObject
{
    public Travel Travel { get; }
    public CurrencyModel[] AllCurrencies { get; }
    
    private readonly SessionService sessionService;
    private readonly AccessManager accessManager;
    
    public TravelDetailViewModel(
        SessionService _sessionService,
        AccessManager _accessManager,
        CurrencyService _currencyService)
    {
        sessionService = _sessionService;
        accessManager = _accessManager;
        
        AllCurrencies = _currencyService.GetAllCurrencyInfos();
        
        //todo exception or load state with nullable
        Travel = sessionService.CurrentTravel!;
    }
    
    [RelayCommand]
    private async Task AddAdditionalCurrency(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return;
        if (Travel.CalculateCurrency == code)
            return;
        if (Travel.AdditionalCurrencies.Contains(code))
            return;
        
        Travel.AdditionalCurrencies.Add(code);
        await sessionService.Save();
    }
    
    [RelayCommand]
    private async Task DeleteAdditionalCurrency(string code)
    {
        //todo delete only when currency is not used
        Travel.AdditionalCurrencies.Remove(code);
        
        await sessionService.Save();
    }
    
    [RelayCommand]
    public async Task AddPerson(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;
        string newId = AccessManager.GetNewId();
        Travel.Participants.Add(new Person(newId, name));
        await sessionService.Save();
    }
    
    //todo delete person when a person doesn't involve any transactions
    //todo make person name editable
}