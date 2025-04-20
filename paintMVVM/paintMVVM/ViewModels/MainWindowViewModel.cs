using System;
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
    public ReactiveCommand<Unit, Unit> LineButtonCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearButtonCommand { get; }
    public ReactiveCommand<PointerEventArgs, Unit> PointerPressedCommand { get; }
    public ReactiveCommand<PointerEventArgs, Unit> PointerMovedCommand { get; }
    public ReactiveCommand<PointerEventArgs, Unit> PointerReleasedCommand { get; }
    public ReactiveCommand<PointerEventArgs, Unit> PointerLeaveCommand { get; }
    private bool _isDrawing;
    private int _currentThickness = 5;
    private Tool _currentTool = Tool.Pencil;
    private Point _startPoint;
    private Point _endPoint;
    private Canvas _canvas;
    private Color _currentColor = Colors.Black;
    private Line _prevLine;
    private Point? _prevPosition;

    public Color CurrentColor
    {
        get => _currentColor;
        set => this.RaiseAndSetIfChanged(ref _currentColor, value);
    }
    
    public int CurrentThickness
    {
        get => _currentThickness;
        set => this.RaiseAndSetIfChanged(ref _currentThickness, value);
    }


    public MainWindowViewModel()
    {
        PencilButtonCommand = ReactiveCommand.Create(PencilButtonClicked);
        LineButtonCommand = ReactiveCommand.Create(LineButtonClicked);
        ClearButtonCommand = ReactiveCommand.Create(ClearButtonClicked);
        PointerPressedCommand = ReactiveCommand.Create<PointerEventArgs>(OnPointerPressed);
        PointerMovedCommand = ReactiveCommand.Create<PointerEventArgs>(OnPointerMoved);
        PointerReleasedCommand = ReactiveCommand.Create<PointerEventArgs>(OnPointerReleased);
        PointerLeaveCommand = ReactiveCommand.Create<PointerEventArgs>(OnPointerLeave);
    }

    public void Initialize(Canvas canvas)
    {
        _canvas = canvas;
    }
    
    private void OnPointerPressed(PointerEventArgs e)
    {
        Debug.Print("PointerPressed", "isDrawing", _isDrawing);
        
        _isDrawing = true;
        _startPoint = e.GetCurrentPoint(_canvas).Position;
    }

    private void OnPointerMoved(PointerEventArgs e)
    {
        Debug.Print("PointerMoved", "isDrawing", _isDrawing);
        
        if (!_isDrawing || _canvas == null)
        {
            _prevPosition = null;
            return;
        }
        
        if (_prevLine != null)
        {
            _canvas.Children.Remove(_prevLine);
        }
        
        var position = e.GetPosition(_canvas);

        if (position.X < 0 || position.Y < 0 || position.X >= _canvas.Width || position.Y >= _canvas.Height)
        {
            return;
        }
        
        switch (_currentTool)
        {
            case Tool.Pencil:
                if (_prevPosition == null)
                {
                    _prevPosition = position;
                }
                var l = new Line
                {
                    StartPoint = _prevPosition.Value,
                    EndPoint = position,
                    Stroke = new SolidColorBrush(CurrentColor),
                    StrokeThickness = CurrentThickness,
                };
                _canvas.Children.Add(l);
                break;
            case Tool.Line:
                var line = new Line
                {
                    StartPoint = _startPoint,
                    EndPoint = position,
                    Stroke = new SolidColorBrush(CurrentColor),
                    StrokeThickness = CurrentThickness,
                };
                _prevLine = line;
                _canvas.Children.Add(line);
                break;
        }

        _prevPosition = position;
    }

    private void OnPointerReleased(PointerEventArgs e)
    {
        Debug.Print("PointerReleased", "isDrawing", _isDrawing);

        if (!_isDrawing || _canvas == null)
        {
            return;
        }
        
        _isDrawing = false;
        
        _endPoint = e.GetCurrentPoint(_canvas).Position;
        
        _endPoint = new Point(
            Math.Clamp(_endPoint.X, 0, _canvas.Bounds.Width),
            Math.Clamp(_endPoint.Y, 0, _canvas.Bounds.Height)
        );

        switch (_currentTool)
        {
            case Tool.Line:
                var line = new Line
                {
                    StartPoint = _startPoint,
                    EndPoint = _endPoint,
                    Stroke = new SolidColorBrush(CurrentColor),
                    StrokeThickness = CurrentThickness,
                };
                
                _canvas.Children.Add(line);
                break;
        }
    }
    
    private void OnPointerLeave(PointerEventArgs e)
    {
        Debug.Print("PointerLeave");
        
        _isDrawing = false;
        _endPoint = e.GetCurrentPoint(_canvas).Position;
    }

    private void PencilButtonClicked()
    {
        Debug.Print("PencilButtonClicked");
        
        _currentTool = Tool.Pencil;
    }
    
    private void LineButtonClicked()
    {
        Debug.Print("LineButtonClicked");
        
        _currentTool = Tool.Line;
    }

    private void ClearButtonClicked()
    {
        Debug.Print("ClearButtonClicked");

        _canvas.Children.Clear();
    }
}
