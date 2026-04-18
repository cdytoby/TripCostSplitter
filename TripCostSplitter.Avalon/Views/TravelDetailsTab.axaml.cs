using Avalonia.Controls;
using TripCostSplitter.AppBase.ViewModels;

namespace TripCostSplitter.Avalon.Views;

public partial class TravelDetailsTab : UserControl
{
    public TravelDetailViewModel? ViewModel { get; }
    
    public TravelDetailsTab()
    {
        InitializeComponent();
    }
    
    public TravelDetailsTab(TravelDetailViewModel _viewModel)
    {
        ViewModel = _viewModel;
        InitializeComponent();
    }
}
