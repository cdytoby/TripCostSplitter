using Avalonia.Controls;
using TripCostSplitter.AppBase.ViewModels;

namespace TripCostSplitter.Avalon.Views;

public partial class SettingsView : ContentPage
{
    public SettingsViewModel? ViewModel { get; }

    public SettingsView()
    {
        InitializeComponent();
    }

    public SettingsView(SettingsViewModel _viewModel)
    {
        ViewModel = _viewModel;
        DataContext = ViewModel;
        InitializeComponent();
    }
}
