using Avalonia.Controls;
using Avalonia.Interactivity;
using TripCostSplitter.AppBase;
using TripCostSplitter.AppBase.Services;
using TripCostSplitter.Avalon.Services;
using TripCostSplitter.Core.Services;

namespace TripCostSplitter.Avalon.Views;

public partial class MainView: UserControl
{
    private INavigationService? navigationService;
    private IDataService? dataService;
    
    public MainView()
    {
        InitializeComponent();
    }
    
    public MainView(
        INavigationService _navigationService,
        IDataService _dataService)
    {
        navigationService = _navigationService;
        dataService = _dataService;
        InitializeComponent();
        
        if (navigationService is AvalonNavigationService myNavigation)
            myNavigation.SetNavigationPage(MainNavigationPage);
    }
    
    protected override async void OnLoaded(RoutedEventArgs e)
    {
        try
        {
            await dataService!.Load();
            await navigationService!.PushAsync(ViewDefinition.TravelListView);
        }
        catch (Exception)
        {
            //ignored
        }
    }
}