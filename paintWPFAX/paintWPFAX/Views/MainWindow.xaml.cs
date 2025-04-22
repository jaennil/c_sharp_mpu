
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using paintWPFAX.Tools;
using paintWPFAX.ViewModels;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace paintWPFAX;


public partial class MainWindow : Window
{
    private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }

    private void DrawingSurface_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        Debug.WriteLine("DrawingSurface_PaintSurface");
        var canvas = e.Surface.Canvas;

        canvas.Clear(SKColors.White);

        //if (ViewModel.Document.Bitmap != null)
        //{
        canvas.DrawBitmap(ViewModel.Document.Bitmap, 0, 0);
        //}

        ViewModel.CurrentTool.OnRender(canvas);

    }

    private void DrawingSurface_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Debug.WriteLine("DrawingSurface_MouseDown");
        DrawingSurface.Focus();
        DrawingSurface.CaptureMouse();
        var position = e.GetPosition(DrawingSurface);
        var point = position.ToSKPoint();
        ViewModel.CurrentTool.OnMouseDown(ViewModel.Document, point, e);
        DrawingSurface.InvalidateVisual();
    }
    
    private void DrawingSurface_MouseMove(object sender, MouseEventArgs e)
    {
        Debug.WriteLine("DrawingSurface_MouseMove");
        var position = e.GetPosition(DrawingSurface);
        var point = position.ToSKPoint();
        ViewModel.CurrentTool.OnMouseMove(ViewModel.Document, point, e);
        DrawingSurface.InvalidateVisual();
    }

    private void DrawingSurface_MouseUp(object sender, MouseButtonEventArgs e)
    {
        Debug.WriteLine("DrawingSurface_MouseUp");
        var position = e.GetPosition(DrawingSurface);
        var point = position.ToSKPoint();
        ViewModel.CurrentTool.OnMouseUp(ViewModel.Document, point, e);
        DrawingSurface.ReleaseMouseCapture();
        DrawingSurface.InvalidateVisual();
    }
}