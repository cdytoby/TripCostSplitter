using Avalonia.Controls;
using TripCostSplitter.AppBase.ViewModels;

namespace TripCostSplitter.Avalon.Views;

public partial class PaymentDetailView: ContentPage
{
    public PaymentDetailViewModel? ViewModel { get; }
    
    public PaymentDetailView()
    {
        InitializeComponent();
    }
    
    public PaymentDetailView(PaymentDetailViewModel _viewModel)
    {
        ViewModel = _viewModel;
        InitializeComponent();
    }
}