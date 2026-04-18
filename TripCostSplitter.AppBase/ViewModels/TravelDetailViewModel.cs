using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.Core;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.AppBase.ViewModels;

public partial class TravelDetailViewModel: ObservableObject
{
    private readonly SessionService sessionService;
    
    public Travel Travel { get; }
    public CurrencyModel[] AllCurrencies { get; }
    
    public TravelDetailViewModel(
        SessionService _sessionService,
        CurrencyService _currencyService)
    {
        sessionService = _sessionService;
        
        AllCurrencies = _currencyService.GetAllCurrencyInfos();
        
        //todo exception or load state with nullable
        Travel = sessionService.CurrentTravel!;
        
        // todo In a real app, we'd load participants from somewhere. For now, let's add some default ones if empty.
        if (Travel.Participants.Count == 0)
        {
            Travel.Participants.Add(new Person(1, "Alice"));
            Travel.Participants.Add(new Person(2, "Bob"));
        }
    }
    
    [RelayCommand]
    private void AddAdditionalCurrency(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return;
        if (Travel.CalculateCurrency == code)
            return;
        if (Travel.AdditionalCurrencies.Contains(code))
            return;
        
        Travel.AdditionalCurrencies.Add(code);
    }
    
    [RelayCommand]
    private void DeleteAdditionalCurrency(string code)
    {
        Travel.AdditionalCurrencies.Remove(code);
    }
    
    [RelayCommand]
    public void AddPerson(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        int newId = Travel.Participants.Count > 0 ? Travel.Participants.Max(p => p.Id) + 1 : 1;
        Travel.Participants.Add(new Person(newId, name));
    }
    
    //todo delete person when a person doesn't involve any transactions
}