using Avalonia.Controls;
using TripCostSplitter.AppBase.ViewModels;

namespace TripCostSplitter.Avalon.Views;

public partial class TravelDetailView: ContentPage
{
    public TravelDetailViewModel? ViewModel { get; }
    
    public TravelDetailView()
    {
        InitializeComponent();
    }
    
    public TravelDetailView(TravelDetailViewModel _viewModel)
    {
        ViewModel = _viewModel;
        InitializeComponent();
    }
}