using Avalonia.Controls;
using TripCostSplitter.AppBase.ViewModels;

namespace TripCostSplitter.Avalon.Views;

public partial class TravelTransactionsTab : UserControl
{
    public TransactionListViewModel? ViewModel { get; }
    
    public TravelTransactionsTab()
    {
        InitializeComponent();
    }
    
    public TravelTransactionsTab(TransactionListViewModel _viewModel)
    {
        ViewModel = _viewModel;
        InitializeComponent();
    }
}
