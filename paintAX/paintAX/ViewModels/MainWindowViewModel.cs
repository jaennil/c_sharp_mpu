using System.Reactive;
using paintAX.Models;
using ReactiveUI;

namespace paintAX.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    public ReactiveCommand<Unit, Unit> PencilCommand { get; }
    public ReactiveCommand<Unit, Unit> LineCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearCommand { get; }
    private Tool _selectedTool = Tool.Pencil;

    public MainWindowViewModel()
    {
        PencilCommand = ReactiveCommand.Create(SelectPencil);
        LineCommand = ReactiveCommand.Create(SelectLine);
        ClearCommand = ReactiveCommand.Create(ClearCanvas);
    }
    
    private void SelectPencil()
    {
        _selectedTool = Tool.Pencil;
    }
    
    private void SelectLine()
    {
        _selectedTool = Tool.Line;
    }

    private void ClearCanvas()
    {
        
    }
}