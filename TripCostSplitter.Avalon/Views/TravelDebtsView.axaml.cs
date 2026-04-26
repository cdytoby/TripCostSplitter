using Avalonia.Controls;
using TripCostSplitter.AppBase.ViewModels;

namespace TripCostSplitter.Avalon.Views;

public partial class TravelDebtsView : UserControl
{
    public DebtsViewModel? ViewModel { get; }
    
    public TravelDebtsView()
    {
        InitializeComponent();
    }
    
    public TravelDebtsView(DebtsViewModel _viewModel)
    {
        ViewModel = _viewModel;
        InitializeComponent();
    }
}
