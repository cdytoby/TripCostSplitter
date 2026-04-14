using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TripCostSplitter.Core;
using TripCostSplitter.Core.DataModels;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.Avalon.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDataService _dataService;

    [ObservableProperty]
    public partial ObservableObject? CurrentViewModel { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<Travel> Travels { get; set; }

    // Parameterless constructor for design time
    public MainViewModel()
    {
        _serviceProvider = null!;
        _dataService = null!;
        Travels = [];
    }

    public MainViewModel(IServiceProvider serviceProvider, IDataService dataService)
    {
        _serviceProvider = serviceProvider;
        _dataService = dataService;
        Travels = [];
    }

    public async Task InitializeAsync()
    {
        IEnumerable<Travel> loadedTravels = await _dataService.LoadAllTravelsAsync();
        Travels = new ObservableCollection<Travel>(loadedTravels);

        if (CurrentViewModel == null)
            CurrentViewModel = _serviceProvider.GetRequiredService<TravelListViewModel>();
    }

    [RelayCommand]
    public async Task SaveData()
    {
        await _dataService.SaveAllTravelsAsync(Travels);
    }

    [RelayCommand]
    public async Task GoBack()
    {
        await _dataService.SaveAllTravelsAsync(Travels);
        CurrentViewModel = _serviceProvider.GetRequiredService<TravelListViewModel>();
    }

    public T CreateViewModel<T>(params object[] parameters) where T : notnull
    {
        return ActivatorUtilities.CreateInstance<T>(_serviceProvider, parameters);
    }
}