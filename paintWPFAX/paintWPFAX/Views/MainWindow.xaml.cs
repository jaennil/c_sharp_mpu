
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
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
        canvas.DrawBitmap(ViewModel.Document.Bitmap, 0, 0);

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
        ViewModel.CurrentPoint = position;

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

    private void ToolButton_Click(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("ToolButton_Click");

        if (sender is FrameworkElement element && element.Tag is ToolBase tool)
        {
            ViewModel.CurrentTool = tool;
        }
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("ClearButton_Click");

        ViewModel.Document.Clear();
        DrawingSurface.InvalidateVisual();
    }

    private void NewMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("NewMenuItem_Click");

        throw new NotImplementedException();
        //DrawingSurface.InvalidateVisual();
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

    private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("SaveMenuItem_Click");

        ViewModel.SaveDocument();
    }

    private void StrokeCapButton_Click(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("StrokeCapButton_Click");

        if (sender is FrameworkElement element && element.Tag is SKStrokeCap sc)
        {
            ViewModel.SelectedStrokeCap = sc;
        }
    }

    private void PaintStyleButton_Click(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("PaintStyleButton_Click");

        if (sender is FrameworkElement element && element.Tag is SKPaintStyle ps)
        {
            ViewModel.SelectedPaintStyle = ps;
        }
    }

    private void ToolRadioButton_Loaded(object sender, RoutedEventArgs e)
    {
        var rb = sender as RadioButton;
        if (rb?.Content.ToString() == ViewModel.CurrentTool.Name)
        {
            rb.IsChecked = true;
        }
    }

    private void StrokeCapRadioButton_Loaded(object sender, RoutedEventArgs e)
    {
        var rb = sender as RadioButton;
        if (rb?.Content.ToString() == ViewModel.CurrentTool.Settings.StrokeCap.ToString())
        {
            rb.IsChecked = true;
        }
    }

    private void PaintStyleRadioButton_Loaded(object sender, RoutedEventArgs e)
    {
        var rb = sender as RadioButton;
        if (rb?.Content.ToString() == ViewModel.CurrentTool.Settings.PaintStyle.ToString())
        {
            rb.IsChecked = true;
        }
    }
}