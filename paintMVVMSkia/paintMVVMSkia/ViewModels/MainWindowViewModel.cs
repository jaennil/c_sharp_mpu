using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Text.Json;
using ReactiveUI;
using SkiaSharp;

public class MainWindowViewModel : ReactiveObject
{
    public enum ToolMode
    {
        Pencil,
        Line,
        Rectangle,
        Ellipse,
        Arrow,
        Star,
        Hexagon,
        Text,
        Select
    }

    private readonly Stack<List<IDrawable>> _undoStack = new();
    private readonly Stack<List<IDrawable>> _redoStack = new();
    private readonly List<IDrawable> _currentLayer = new();
    private readonly List<List<IDrawable>> _layers = new();
    private SKPoint _startPoint;
    private SKPoint _lastPoint;
    private ToolMode _currentTool = ToolMode.Pencil;
    private IDrawable? _currentShape;
    private SKRect _selectionRect;
    private bool _isSelecting;
    private bool _isMovingSelection;
    private SKColor _pixelColorUnderCursor = SKColors.Transparent;
    
    [Reactive] public SKColor StrokeColor { get; set; } = SKColors.Black;
    [Reactive] public SKColor FillColor { get; set; } = SKColors.Transparent;
    [Reactive] public float StrokeWidth { get; set; } = 3f;
    [Reactive] public string StatusText { get; set; } = "Ready";
    [Reactive] public string FontFamily { get; set; } = "Arial";
    [Reactive] public float FontSize { get; set; } = 24f;
    [Reactive] public int CurrentLayerIndex { get; set; }
    [Reactive] public bool ShowGrid { get; set; }
    
    public ObservableCollection<ToolMode> AvailableTools { get; } = new(Enum.GetValues<ToolMode>());
    public ReactiveCommand<Unit, Unit> UndoCommand { get; }
    public ReactiveCommand<Unit, Unit> RedoCommand { get; }
    public ReactiveCommand<Unit, Unit> CopyCommand { get; }
    public ReactiveCommand<Unit, Unit> PasteCommand { get; }
    public ReactiveCommand<Unit, Unit> AddLayerCommand { get; }
    public ReactiveCommand<Unit, Unit> RemoveLayerCommand { get; }

    public MainWindowViewModel()
    {
        _layers.Add(new List<IDrawable>());
        
        UndoCommand = ReactiveCommand.Create(Undo, this.WhenAnyValue(x => x._undoStack.Count, count => count > 0));
        RedoCommand = ReactiveCommand.Create(Redo, this.WhenAnyValue(x => x._redoStack.Count, count => count > 0));
        CopyCommand = ReactiveCommand.Create(CopySelection);
        PasteCommand = ReactiveCommand.Create(PasteFromClipboard);
        AddLayerCommand = ReactiveCommand.Create(AddLayer);
        RemoveLayerCommand = ReactiveCommand.Create(RemoveLayer, 
            this.WhenAnyValue(x => x._layers.Count, count => count > 1));
    }

    public void HandlePointerPressed(SKPoint point)
    {
        SaveState();
        
        _startPoint = point;
        _lastPoint = point;
        
        switch (_currentTool)
        {
            case ToolMode.Select:
                _isSelecting = true;
                _selectionRect = new SKRect(point.X, point.Y, point.X, point.Y);
                break;
                
            case ToolMode.Text:
                // Show text input dialog
                var text = ShowTextInputDialog();
                if (!string.IsNullOrEmpty(text))
                {
                    _currentLayer.Add(new TextDrawable
                    {
                        Text = text,
                        Position = point,
                        FontFamily = FontFamily,
                        FontSize = FontSize,
                        Color = StrokeColor
                    });
                }
                break;
                
            default:
                _currentShape = CreateShape(_currentTool);
                break;
        }
    }

    public void HandlePointerMoved(SKPoint point)
    {
        _lastPoint = point;
        
        if (_isSelecting)
        {
            _selectionRect = SKRect.Create(_startPoint.X, _startPoint.Y, 
                point.X - _startPoint.X, point.Y - _startPoint.Y);
        }
        else if (_isMovingSelection)
        {
            var dx = point.X - _lastPoint.X;
            var dy = point.Y - _lastPoint.Y;
            
            foreach (var shape in GetSelectedShapes())
            {
                shape.Move(dx, dy);
            }
        }
        else if (_currentShape != null)
        {
            _currentShape.Update(_startPoint, point);
        }
        
        UpdateStatusText(point);
    }

    public void HandlePointerReleased(SKPoint point)
    {
        if (_currentShape != null)
        {
            _currentLayer.Add(_currentShape);
            _currentShape = null;
        }
        
        _isSelecting = false;
        _isMovingSelection = false;
    }

    public void Draw(SKCanvas canvas, SKImageInfo info)
    {
        canvas.Clear(SKColors.White);
        
        // Draw grid if enabled
        if (ShowGrid)
        {
            DrawGrid(canvas, info);
        }
        
        // Draw all layers
        foreach (var layer in _layers)
        {
            foreach (var shape in layer)
            {
                shape.Draw(canvas);
            }
        }
        
        // Draw current shape in progress
        _currentShape?.Draw(canvas);
        
        // Draw selection rectangle
        if (_isSelecting || _isMovingSelection)
        {
            using var paint = new SKPaint
            {
                Color = new SKColor(100, 100, 255, 100),
                Style = SKPaintStyle.Fill
            };
            canvas.DrawRect(_selectionRect, paint);
            
            paint.Style = SKPaintStyle.Stroke;
            paint.Color = SKColors.Blue;
            paint.StrokeWidth = 1;
            canvas.DrawRect(_selectionRect, paint);
        }
    }

    private IDrawable CreateShape(ToolMode tool)
    {
        return tool switch
        {
            ToolMode.Pencil => new PencilDrawable { Color = StrokeColor, StrokeWidth = StrokeWidth },
            ToolMode.Line => new LineDrawable { Color = StrokeColor, StrokeWidth = StrokeWidth },
            ToolMode.Rectangle => new RectangleDrawable 
                { StrokeColor = StrokeColor, FillColor = FillColor, StrokeWidth = StrokeWidth },
            ToolMode.Ellipse => new EllipseDrawable 
                { StrokeColor = StrokeColor, FillColor = FillColor, StrokeWidth = StrokeWidth },
            ToolMode.Arrow => new ArrowDrawable 
                { Color = StrokeColor, StrokeWidth = StrokeWidth },
            ToolMode.Star => new StarDrawable(5) 
                { StrokeColor = StrokeColor, FillColor = FillColor, StrokeWidth = StrokeWidth },
            ToolMode.Hexagon => new PolygonDrawable(6) 
                { StrokeColor = StrokeColor, FillColor = FillColor, StrokeWidth = StrokeWidth },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void SaveState()
    {
        _undoStack.Push(new List<IDrawable>(_currentLayer));
        _redoStack.Clear();
    }

    private void Undo()
    {
        if (_undoStack.Count == 0) return;
        
        _redoStack.Push(new List<IDrawable>(_currentLayer));
        _currentLayer.Clear();
        _currentLayer.AddRange(_undoStack.Pop());
    }

    private void Redo()
    {
        if (_redoStack.Count == 0) return;
        
        _undoStack.Push(new List<IDrawable>(_currentLayer));
        _currentLayer.Clear();
        _currentLayer.AddRange(_redoStack.Pop());
    }

    private void CopySelection()
    {
        var selected = GetSelectedShapes();
        if (selected.Count == 0) return;
        
        // Serialize selected shapes to clipboard
        var json = JsonSerializer.Serialize(selected);
        Application.Current.Clipboard.SetTextAsync(json);
    }

    private void PasteFromClipboard()
    {
        try
        {
            var json = await Application.Current.Clipboard.GetTextAsync();
            if (string.IsNullOrEmpty(json)) return;
            
            var shapes = JsonSerializer.Deserialize<List<IDrawable>>(json);
            if (shapes == null || shapes.Count == 0) return;
            
            SaveState();
            _currentLayer.AddRange(shapes);
        }
        catch { /* Handle error */ }
    }

    private void AddLayer()
    {
        _layers.Add(new List<IDrawable>());
        CurrentLayerIndex = _layers.Count - 1;
    }

    private void RemoveLayer()
    {
        if (_layers.Count <= 1) return;
        
        _layers.RemoveAt(CurrentLayerIndex);
        CurrentLayerIndex = Math.Min(CurrentLayerIndex, _layers.Count - 1);
    }

    private List<IDrawable> GetSelectedShapes()
    {
        return _currentLayer.Where(shape => _selectionRect.Contains(shape.Bounds)).ToList();
    }

    private void UpdateStatusText(SKPoint point)
    {
        // Get pixel color under cursor (would need bitmap access)
        // _pixelColorUnderCursor = GetPixelColor(point);
        
        StatusText = $"X: {point.X:F1}, Y: {point.Y:F1} | " +
                    $"Color: {_pixelColorUnderCursor} | " +
                    $"Tool: {_currentTool} | " +
                    $"Layer: {CurrentLayerIndex + 1}/{_layers.Count}";
    }

    // ... other helper methods ...
}