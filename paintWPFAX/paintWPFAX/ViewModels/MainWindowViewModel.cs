using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using paintWPFAX.Models;
using paintWPFAX.Tools;

namespace paintWPFAX.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private ITool _currentTool;
    private ToolSettings _toolSettings;
    private DrawingDocument _document;

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
        _toolSettings = new ToolSettings();

        CurrentTool = new PencilTool(_toolSettings);

                                       // FIX ME
        Document = new DrawingDocument(800, 600);
    }
}
