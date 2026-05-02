using Avalonia.Controls;
using TripCostSplitter.AppBase.ViewModels;

namespace TripCostSplitter.Avalon.Views;

public partial class TravelDetailsView: UserControl
{
    public TravelDetailViewModel? ViewModel { get; }
    
    public TravelDetailsView()
    {
        InitializeComponent();
    }
    
    public TravelDetailsView(TravelDetailViewModel _viewModel)
    {
        ViewModel = _viewModel;
        DataContext = _viewModel;
        InitializeComponent();
    }
    
    private async void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (ViewModel == null)
            return;
        
        await ViewModel.Save();
    }
}