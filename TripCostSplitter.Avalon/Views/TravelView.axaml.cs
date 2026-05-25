using Avalonia.Controls;
using TripCostSplitter.AppBase.Services;

namespace TripCostSplitter.Avalon.Views;

public partial class TravelView: ContentPage
{
    private TravelDebtsView? travelDebtsView;
    private TravelTransactionsView? travelTransactionsView;
    
    public TravelView()
    {
        InitializeComponent();
    }
    
    public TravelView(
        SessionService _sessionService,
        TravelDetailsView _travelDetailsView,
        TravelTransactionsView _travelTransactionsView,
        TravelDebtsView _travelDebtsView)
    {
        InitializeComponent();
        
        Header = _sessionService.CurrentTravel?.Name;
        
        DetailsTab.Content = _travelDetailsView;
        TransactionsTab.Content = _travelTransactionsView;
        DebtsTab.Content = _travelDebtsView;
        
        travelDebtsView = _travelDebtsView;
        travelTransactionsView = _travelTransactionsView;
    }
    
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        travelTransactionsView?.ViewModel?.RefreshTransactions();
        travelDebtsView?.ViewModel?.UpdateDebtsCommand.Execute(null);
    }
    
    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        TabItem[] addedTabItems = e.AddedItems.OfType<TabItem>().ToArray();
        if (addedTabItems.Length == 0)
            return;
        TabItem tabItem = addedTabItems[0];
        
        if (tabItem == TransactionsTab)
        {
            NavigationPage.SetTopCommandBar(this, travelTransactionsView?.FindResource("TopBar") as Control);
            travelTransactionsView?.ViewModel?.RefreshTransactions();
        }
        else if (tabItem == DebtsTab)
        {
            NavigationPage.SetTopCommandBar(this, travelDebtsView?.FindResource("TopBar") as Control);
            travelDebtsView?.ViewModel?.UpdateDebtsCommand.Execute(null);
        }
        else if( tabItem == DetailsTab)
        {
            NavigationPage.SetTopCommandBar(this, null);
        }
    }
}