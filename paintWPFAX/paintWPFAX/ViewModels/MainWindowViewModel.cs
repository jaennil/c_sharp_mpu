using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using paintWPFAX.Models;
using paintWPFAX.Services;
using paintWPFAX.Tools;
using SkiaSharp;
using SkiaSharp.Views.WPF;

namespace paintWPFAX.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private ToolBase _currentTool;
    private ToolSettings _toolSettings;
    private DrawingDocument _document;
    private FileService _fileService;
    private ObservableCollection<SKStrokeCap> _skStrokeCaps;
    private ObservableCollection<SKPaintStyle> _skPaintStyles;
    private ObservableCollection<ToolBase> _tools;
    private Point _currentPoint; 
    private string _windowTitle;

    public ObservableCollection<SKStrokeCap> SKStrokeCaps => _skStrokeCaps;
    public ObservableCollection<SKPaintStyle> SKPaintStyles => _skPaintStyles;
    public ObservableCollection<ToolBase> Tools => _tools;
    public Point CurrentPoint
    {
        get => _currentPoint;
        set
        {
            _currentPoint = new Point((int)value.X, (int)value.Y);
            OnPropertyChanged(nameof(CurrentPoint));
        }
    }

    public MainWindowViewModel()
    {
        _fileService = new FileService();
        // FIX ME
        Document = new DrawingDocument(800, 600);

        _skStrokeCaps = new ObservableCollection<SKStrokeCap>
        {
            SKStrokeCap.Butt,
            SKStrokeCap.Round,
            SKStrokeCap.Square
        };

        _skPaintStyles = new ObservableCollection<SKPaintStyle>
        {
            SKPaintStyle.Fill,
            SKPaintStyle.Stroke,
            SKPaintStyle.StrokeAndFill
        };

        _tools = new ObservableCollection<ToolBase>
        {
            new PencilTool(new ToolSettings()),
            new EraserTool(new ToolSettings()),
            new LineTool(new ToolSettings()),
            new OvalTool(new ToolSettings()),
            new RectangleTool(new ToolSettings()),
            new RoundRectangleTool(new ToolSettings()),
            new TextTool(new ToolSettings()),
            new SelectionTool(new ToolSettings()),
        };
        CurrentTool = _tools[0];
    }

    public SKStrokeCap SelectedStrokeCap
    {
        get => CurrentTool.Settings.StrokeCap;
        set
        {
            CurrentTool.Settings.StrokeCap = value;
            OnPropertyChanged(nameof(SelectedStrokeCap));
        }
    }

    public SKPaintStyle SelectedPaintStyle
    {
        get => CurrentTool.Settings.PaintStyle;
        set
        {
            CurrentTool.Settings.PaintStyle = value;
            OnPropertyChanged(nameof(SelectedPaintStyle));
        }
    }

    public string WindowTitle
    {
        get
        {
            _windowTitle = $"{Document.DisplayName} - Paint";
            return _windowTitle;
        }
        set
        {
            _windowTitle = value;
        }
    }

    public Color CurrentColor
    {
        get => CurrentTool.Settings.StrokeColor.ToColor();
        set
        {
            CurrentTool.Settings.StrokeColor = value.ToSKColor();
        }
    }

    public float StrokeWidth
    {
        get => CurrentTool.Settings.StrokeWidth;
        set
        {
            CurrentTool.Settings.StrokeWidth = value;
        }
    }

    public bool IsAntialias
    {
        get => CurrentTool.Settings.IsAntialias;
        set
        {
            CurrentTool.Settings.IsAntialias = value;
        }
    }

    public ToolBase CurrentTool
    {
        get => _currentTool;
        set
        {
            _currentTool = value;
            OnPropertyChanged(nameof(CurrentTool));
            OnPropertyChanged(nameof(CurrentColor));
            OnPropertyChanged(nameof(StrokeWidth));
            OnPropertyChanged(nameof(IsAntialias));
            OnPropertyChanged(nameof(SelectedStrokeCap));
            OnPropertyChanged(nameof(SelectedPaintStyle));
        }
    }

    public DrawingDocument Document
    {
        get => _document;
        private set
        {
            _document = value;
        }
    }

    public async void OpenDocument()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpg;*.jpeg)|*.jpg;*.jpeg",
            Title = "Open Image"
        };

        if (dialog.ShowDialog() == false) return;

        var filePath = dialog.FileName;
        var width = Document.Width;
        var height = Document.Height;
        var newDocument = await _fileService.OpenDocumentAsync(filePath, width, height);
        Document = newDocument;
        OnPropertyChanged(nameof(WindowTitle));
    }

    public async void SaveDocument()
    {
        if (Document == null) return;

        if (string.IsNullOrEmpty(Document.FilePath))
        {
            SaveDocumentAs();
        } else
        {
            await _fileService.SaveDocumentAsync(Document, Document.FilePath);
        }
    }

    public async void SaveDocumentAs()
    {
        if (Document == null) return;
        var dialog = new SaveFileDialog
        {
            Filter = "PNG Files (*.png)|*.png",
            Title = "Save Image As"
        };

        if (dialog.ShowDialog() == false) return;

        var filePath = dialog.FileName;
        await _fileService.SaveDocumentAsync(Document, filePath);
        OnPropertyChanged(nameof(WindowTitle));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
