using System.Diagnostics;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using paintMVVM.Models;
using ReactiveUI;

namespace paintMVVM.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> PencilButtonCommand { get; }
    private bool _isDrawing;
    private Tool _currentTool = Tool.Pencil;
    private Point _startPoint;
    private Point _endPoint;


    public MainWindowViewModel()
    {
        PencilButtonCommand = ReactiveCommand.Create(PencilButtonClicked);
    }
    
    private void OnPointerPressed(PointerPressedEventArgs e)
    {
        Debug.Print("PointerPressed");
        
        _isDrawing = true;
        _startPoint = e.GetCurrentPoint(DrawingCanvas).Position;
    }

    private void OnPointerMoved(PointerEventArgs e)
    {
        Debug.Print("PointerMoved");
        
        if (!_isDrawing)
        {
            return;
        }

        switch (_currentTool)
        {
            case Tool.Pencil:
                var ellipse = new Ellipse
                {
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 3,
                    Fill = new SolidColorBrush(Colors.Black),
                    Width = 5,
                    Height = 5,
                };
                Canvas.SetLeft(ellipse, e.GetPosition(DrawingCanvas).X);
                Canvas.SetTop(ellipse, e.GetPosition(DrawingCanvas).Y);
                DrawingCanvas.Children.Add(ellipse);
                break;
        }
        
    }

    private void OnPointerReleased(PointerReleasedEventArgs e)
    {
        Debug.Print("PointerReleased");
        
        _isDrawing = false;
        _endPoint = e.GetCurrentPoint(DrawingCanvas).Position;

        switch (_currentTool)
        {
            case Tool.Line:
                var line = new Line
                {
                    StartPoint = _startPoint,
                    EndPoint = _endPoint,
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 3,
                };
                
                DrawingCanvas.Children.Add(line);
                break;
        }
    }

    private void PencilButtonClicked()
    {
        Debug.Print("PencilButtonClicked");
        
        _currentTool = Tool.Pencil;
    }
    
    private void LineButtonClicked()
    {
        Debug.Print("PencilButtonClicked");
        
        _currentTool = Tool.Pencil;
    }
}