using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TripCostSplitter.Core;
using TripCostSplitter.Core.DataModels;

namespace TripCostSplitter.Avalon.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITravelDataService _dataService;

    [ObservableProperty]
    public partial ObservableObject? CurrentViewModel { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<Travel> Travels { get; set; }

    // Parameterless constructor for design time
    public MainViewModel()
    {
        _serviceProvider = null!;
        _dataService = null!;
        Travels = new();
    }

    public MainViewModel(IServiceProvider serviceProvider, ITravelDataService dataService)
    {
        _serviceProvider = serviceProvider;
        _dataService = dataService;
        Travels = new();
    }

    public async Task InitializeAsync()
    {
        var loadedTravels = await _dataService.LoadAsync();
        Travels = new ObservableCollection<Travel>(loadedTravels);

        if (CurrentViewModel == null)
            CurrentViewModel = _serviceProvider.GetRequiredService<TravelListViewModel>();
    }

    [RelayCommand]
    public async Task SaveData()
    {
        await _dataService.SaveAsync(Travels);
    }

    [RelayCommand]
    public async Task GoBack()
    {
        await _dataService.SaveAsync(Travels);
        CurrentViewModel = _serviceProvider.GetRequiredService<TravelListViewModel>();
    }

    public T ResolveViewModel<T>() where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    public T CreateViewModel<T>(params object[] parameters) where T : notnull
    {
        return ActivatorUtilities.CreateInstance<T>(_serviceProvider, parameters);
    }
}