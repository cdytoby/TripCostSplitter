using Avalonia.Controls;
using TripCostSplitter.AppBase.Services;

namespace TripCostSplitter.Avalon.Views;

public partial class TravelView: ContentPage
{
    private TravelDebtsTab? travelDebtsTab;
    
    public TravelView()
    {
        InitializeComponent();
    }
    
    public TravelView(
        SessionService _sessionService,
        TravelDetailsTab _travelDetailsTab,
        TravelTransactionsTab _travelTransactionsTab,
        TravelDebtsTab _travelDebtsTab)
    {
        InitializeComponent();
        
        Header = _sessionService.CurrentTravel?.Name;
        
        DetailsTab.Content = _travelDetailsTab;
        TransactionsTab.Content = _travelTransactionsTab;
        DebtsTab.Content = _travelDebtsTab;
        
        travelDebtsTab = _travelDebtsTab;
    }
    
    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        travelDebtsTab?.ViewModel?.UpdateDebts();
    }
}