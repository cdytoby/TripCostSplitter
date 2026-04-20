using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using TripCostSplitter.AppBase;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.Avalon.Views;

namespace TripCostSplitter.Avalon.Services;

public class AvalonNavigationService: INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private NavigationPage? _navigationPage;
    
    public AvalonNavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public void SetNavigationPage(NavigationPage navigationPage)
    {
        _navigationPage = navigationPage;
    }
    
    public async Task PushAsync(string pageId)
    {
        if (_navigationPage == null)
            throw new InvalidOperationException("NavigationPage not set.");
        
        Page view = GetPageFromId(pageId);
        await _navigationPage.PushAsync(view);
    }
    
    public async Task PopAsync()
    {
        if (_navigationPage == null)
            throw new InvalidOperationException("NavigationPage not set.");
        
        await _navigationPage.PopAsync();
    }
    
    private Page GetPageFromId(string pageId)
    {
        return pageId switch
        {
            ViewDefinition.TravelListView => _serviceProvider.GetRequiredService<TravelListView>(),
            ViewDefinition.TravelDetailView => _serviceProvider.GetRequiredService<TravelView>(),
            ViewDefinition.PaymentDetailView => _serviceProvider.GetRequiredService<PaymentDetailView>(),
            _ => throw new Exception($"Unknown pageId: {pageId}")
        };
    }
}