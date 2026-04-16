using Avalonia.Controls;
using TripCostSplitter.AppBase;
using TripCostSplitter.AppBase.ViewModels;

namespace TripCostSplitter.Avalon.Views;

public partial class TravelListView: ContentPage
{
    public TravelListViewModel? ViewModel { get; }
    
    public TravelListView()
    {
        InitializeComponent();
    }
    
    public TravelListView(TravelListViewModel _viewModel)
    {
        ViewModel = _viewModel;
        InitializeComponent();
    }
}