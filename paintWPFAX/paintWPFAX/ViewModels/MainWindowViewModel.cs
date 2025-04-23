using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using paintWPFAX.Models;
using paintWPFAX.Services;
using paintWPFAX.Tools;

namespace paintWPFAX.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private ITool _currentTool;
    public PencilTool PencilTool;
    public EraserTool EraserTool;
    public LineTool LineTool;
    private ToolSettings _toolSettings;
    private DrawingDocument _document;
    private FileService _fileService;

    public event PropertyChangedEventHandler PropertyChanged;
    public string WindowTitle => $"{Document.DisplayName} - Paint";

    public ITool CurrentTool
    {
        get => _currentTool;
        set
        {
            _currentTool = value;
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

    public MainWindowViewModel()
    {
        PencilTool = new PencilTool(new ToolSettings());
        EraserTool = new EraserTool(new ToolSettings());
        LineTool = new LineTool(new ToolSettings());
        CurrentTool = PencilTool;

        _fileService = new FileService();
                                       // FIX ME
        Document = new DrawingDocument(800, 600);
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
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
