using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using paintMVVM.ViewModels;

namespace paintMVVM.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        
        ViewModel = new MainWindowViewModel();
    }

    private void DrawingCanvas_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}