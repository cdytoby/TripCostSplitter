using Avalonia.Controls;
using TripCostSplitter.AppBase.ViewModels;

namespace TripCostSplitter.Avalon.Views;

public partial class TravelDebtsTab : UserControl
{
    public DebtsViewModel? ViewModel { get; }
    
    public TravelDebtsTab()
    {
        InitializeComponent();
    }
    
    public TravelDebtsTab(DebtsViewModel _viewModel)
    {
        ViewModel = _viewModel;
        InitializeComponent();
    }
}
