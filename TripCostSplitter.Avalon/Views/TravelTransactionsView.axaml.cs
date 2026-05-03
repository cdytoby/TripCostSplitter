using Avalonia.Controls;
using TripCostSplitter.AppBase.ViewModels;

namespace TripCostSplitter.Avalon.Views;

public partial class TravelTransactionsView : UserControl
{
    public TransactionListViewModel? ViewModel { get; }
    
    public TravelTransactionsView()
    {
        InitializeComponent();
    }
    
    public TravelTransactionsView(TransactionListViewModel _viewModel)
    {
        ViewModel = _viewModel;
        DataContext = _viewModel;
        InitializeComponent();
    }
}
