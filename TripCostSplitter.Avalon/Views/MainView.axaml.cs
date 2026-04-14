using Avalonia.Controls;
using TripCostSplitter.Avalon.Services;
using TripCostSplitter.Avalon.ViewModels;

namespace TripCostSplitter.Avalon.Views;

public partial class MainView : NavigationPage
{
    public MainView()
    {
        InitializeComponent();
    }

    public MainView(NavigationService navigationService)
    {
        InitializeComponent();
        navigationService.SetNavigationPage(this);
    }
}