using Avalonia.Controls;
using TripCostSplitter.AppBase.Services;

namespace TripCostSplitter.Avalon.Views;

public partial class TravelView: ContentPage
{
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
    }
}