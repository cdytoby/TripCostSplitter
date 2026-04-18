using Avalonia.Controls;
using TripCostSplitter.AppBase;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.Avalon.Services;

namespace TripCostSplitter.Avalon.Views;

public partial class MainView: UserControl
{
    public MainView()
    {
        InitializeComponent();
    }
    
    public MainView(INavigationService navigationService)
    {
        InitializeComponent();
        if (navigationService is AvalonNavigationService myNavigation)
            myNavigation.SetNavigationPage(MainNavigationPage);
        
        navigationService.PushAsync(ViewDefinition.TravelListView);
    }
}