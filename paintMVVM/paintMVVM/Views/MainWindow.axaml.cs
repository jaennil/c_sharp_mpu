using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using paintMVVM.ViewModels;

namespace paintMVVM.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    
    private MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;
    public MainWindow()
    {
        InitializeComponent();

        Loaded += (sender, e) =>
        {
            if (ViewModel != null)
            {
                ViewModel.Initialize(DrawingCanvas);
            }
        };
    }
}