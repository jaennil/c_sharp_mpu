
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

    private void PencilButton_Click(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("PencilButton_Click");

        // FIX ME
        ViewModel.CurrentTool = ViewModel.PencilTool;
    }

    private void EraserButton_Click(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("EraserButton_Click");

        // FIX ME
        ViewModel.CurrentTool = ViewModel.EraserTool;
    }

    private void LineButton_Click(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("LineButton_Click");

        // FIX ME
        ViewModel.CurrentTool = ViewModel.LineTool;
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("ClearButton_Click");

        ViewModel.Document.Clear();
        DrawingSurface.InvalidateVisual();
    }

    private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("OpenMenuItem_Click");

        ViewModel.OpenDocument();
        DrawingSurface.InvalidateVisual();
    }
    private void SaveAsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("SaveAsMenuItem_Click");

        ViewModel.SaveDocumentAs();
    }
}