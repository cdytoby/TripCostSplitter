using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using TripCostSplitter.Avalon.ViewModels;
using TripCostSplitter.Avalon.Views;

namespace TripCostSplitter.Avalon.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private NavigationPage? _navigationPage;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void SetNavigationPage(NavigationPage navigationPage)
    {
        _navigationPage = navigationPage;
    }

    public async Task PushAsync<TViewModel>(params object[] parameters) where TViewModel : class
    {
        if (_navigationPage == null) throw new InvalidOperationException("NavigationPage not set.");

        TViewModel viewModel = ActivatorUtilities.CreateInstance<TViewModel>(_serviceProvider, parameters);
        Page view = GetViewForViewModel(viewModel);
        view.DataContext = viewModel;
        await _navigationPage.PushAsync(view);
    }

    public async Task PopAsync()
    {
        if (_navigationPage == null) throw new InvalidOperationException("NavigationPage not set.");
        await _navigationPage.PopAsync();
    }

    private Page GetViewForViewModel(object viewModel)
    {
        return viewModel switch
        {
            TravelListViewModel => (Page)_serviceProvider.GetRequiredService<TravelListView>(),
            TravelDetailViewModel => (Page)_serviceProvider.GetRequiredService<TravelDetailView>(),
            TransactionDetailViewModel => (Page)_serviceProvider.GetRequiredService<TransactionDetailView>(),
            DebtResultViewModel => (Page)_serviceProvider.GetRequiredService<DebtResultView>(),
            _ => throw new Exception($"Unknown view model type: {viewModel.GetType()}")
        };
    }
}
