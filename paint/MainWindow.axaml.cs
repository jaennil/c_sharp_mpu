using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace paint;

enum Tool
{
    Pencil,
    Line,
}

public partial class MainWindow : Window
{
    private bool _isDrawing;
    private Tool _currentTool = Tool.Pencil;
    private Point _startPoint;
    private Point _endPoint;
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        Debug.Print("PointerPressed");
        
        _isDrawing = true;
        _startPoint = e.GetCurrentPoint(DrawingCanvas).Position;
    }

    private void OnPointerMoved(object sender, PointerEventArgs e)
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

    private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
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

    private void OnPencilButtonClick(object sender, RoutedEventArgs e)
    {
        Debug.Print("onPencilButtonClick");
        
        _currentTool = Tool.Pencil;
    }

    private void OnLineButtonClick(object sender, RoutedEventArgs e)
    {
        Debug.Print("onLineButtonClick");
        
        _currentTool = Tool.Line;
    }

    private void OnClearButtonClick(object sender, RoutedEventArgs e)
    {
        Debug.Print("onClearButtonClick");
        
        DrawingCanvas.Children.Clear();
    }
}