using Avalonia.Controls;
using TripCostSplitter.AppBase.ViewModels;

namespace TripCostSplitter.Avalon.Views;

public partial class TransferDetailView: ContentPage
{
    public TransferDetailViewModel? ViewModel { get; }
    
    public TransferDetailView()
    {
        InitializeComponent();
    }
    
    public TransferDetailView(TransferDetailViewModel _viewModel)
    {
        ViewModel = _viewModel;
        DataContext = _viewModel;
        InitializeComponent();
    }
}
