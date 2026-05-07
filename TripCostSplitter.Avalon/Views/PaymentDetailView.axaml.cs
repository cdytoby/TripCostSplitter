using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.ComponentModel;
using TripCostSplitter.AppBase.ViewModels;
using TripCostSplitter.Avalon.Converters;
using TripCostSplitter.Core.Services;

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
        DataContext = _viewModel;
        InitializeComponent();
    }
    
    private void PaymentPriceInputElement_OnLostFocus(object? sender, FocusChangedEventArgs e)
    {
        ViewModel?.PaymentPriceUpdated();
    }
    
    private void PurchaseItemInputElement_OnLostFocus(object? sender, FocusChangedEventArgs e)
    {
        ViewModel?.PurchaseItemUpdated();
    }
    
    private void PurchaseItemPriceInputElement_OnLostFocus(object? sender, FocusChangedEventArgs e)
    {
        ViewModel?.PurchaseItemUpdated();
    }
}