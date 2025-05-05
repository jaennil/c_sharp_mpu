using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using cards.Models;
using cards.ViewModels;

namespace cards.Views;

public partial class MainWindow : Window
{
    // Track the initial pointer position and card position when drag starts
    private Point? _dragStartPoint;
    private double _dragStartCardX;
    private double _dragStartCardY;
    private Card? _currentDragCard;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Image { Tag: Card card })
        {
            var point = e.GetCurrentPoint(this);
            if (point.Properties.IsLeftButtonPressed)
            {
                e.Pointer.Capture((IInputElement)sender);
                
                _dragStartPoint = point.Position;
                _dragStartCardX = card.X;
                _dragStartCardY = card.Y;
                _currentDragCard = card;
                
                if (DataContext is MainWindowViewModel viewModel)
                {
                    double maxZ = viewModel.DisplayedCards.Max(c => c.ZIndex);
                    Debug.WriteLine(maxZ);
                    card.ZIndex = maxZ + 1;
                }
            }
        }
    }

    private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_dragStartPoint != null && _currentDragCard != null)
        {
            var point = e.GetCurrentPoint(this);
            if (point.Properties.IsLeftButtonPressed)
            {
                var delta = point.Position - _dragStartPoint.Value;
                _currentDragCard.X = _dragStartCardX + delta.X;
                _currentDragCard.Y = _dragStartCardY + delta.Y;
            }
        }
    }

    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Image image)
        {
            e.Pointer.Capture(null);
            _dragStartPoint = null;
            _currentDragCard = null;
        }
    }
}