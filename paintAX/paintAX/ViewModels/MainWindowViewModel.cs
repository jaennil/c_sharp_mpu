using System.Reactive;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using paintAX.Models;
using ReactiveUI;
using System;
using System.Diagnostics;
using Avalonia;

namespace paintAX.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    public ReactiveCommand<Unit, Unit> PencilCommand { get; }
    public ReactiveCommand<Unit, Unit> LineCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearCommand { get; }
    public ReactiveCommand<PointerEventArgs, Unit> PointerPressedCommand { get; }
    private WriteableBitmap _image;

    public WriteableBitmap Image
    {
        get => _image;
        set => this.RaiseAndSetIfChanged(ref _image, value);
    }
    private Tool _selectedTool = Tool.Pencil;

    public MainWindowViewModel()
    {
        PencilCommand = ReactiveCommand.Create(SelectPencil);
        LineCommand = ReactiveCommand.Create(SelectLine);
        ClearCommand = ReactiveCommand.Create(ClearCanvas);
        PointerPressedCommand = ReactiveCommand.Create<PointerEventArgs>(PointerPressed);
        _image = new WriteableBitmap(new PixelSize(500, 500), new Vector(96, 96), PixelFormat.Rgba8888);
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
    
    public void PointerPressed(PointerEventArgs e)
    {
        Debug.WriteLine("Pointer pressed");
    }
}