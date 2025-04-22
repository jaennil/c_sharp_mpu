
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using SkiaSharp;
using SkiaSharp.Views.WPF;

namespace paintWPFAX
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private SKBitmap _bitmap;
        private SKCanvas _canvas;
        private bool _isDrawing;
        private SKPoint _startPoint;
        private SKPoint _endPoint;
        private Tool _currentTool = Tool.Pencil;
        private string _windowTitle = "Untitled - Paint";

        private SKRect _selectionRect;
        private SKBitmap _selectionBitmap;
        private bool _hasSelection;
        private bool _isMovingSelection;
        private SKPoint _selectionOffset;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                _windowTitle = value;
                OnPropertyChanged(nameof(WindowTitle));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void InitializeBitmap()
        {
            Debug.WriteLine("InitializeBitmap");

            _bitmap = new SKBitmap(
                (int)DrawingSurface.ActualWidth,
                (int)DrawingSurface.ActualHeight,
                SKColorType.Rgba8888,
                SKAlphaType.Premul
            );

            _canvas = new SKCanvas(_bitmap);
            _canvas.Clear(SKColors.White);
            DrawingSurface.InvalidateVisual();
        }

        private void DrawingSurface_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            Debug.WriteLine("DrawingSurface_PaintSurface");

            if (_bitmap == null)
            {
                InitializeBitmap();
                return;
            }

            var surface = e.Surface;
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.White);
            canvas.DrawBitmap(_bitmap, 0, 0);

            if (!_isDrawing && !_hasSelection) return;

            if (_hasSelection && _selectionBitmap != null)
            {
                // Draw the selected area at its current position
                canvas.DrawBitmap(_selectionBitmap, _selectionRect.Left, _selectionRect.Top);

                // Draw a dashed rectangle around the selection
                using (var paint = new SKPaint
                {
                    Color = SKColors.Blue,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 1,
                    PathEffect = SKPathEffect.CreateDash(new float[] { 5, 5 }, 0)
                })
                {
                    canvas.DrawRect(_selectionRect, paint);
                }
            }
            else if (_isDrawing)
            {
                switch (_currentTool)
                {
                    case Tool.Line:
                        using (var paint = GetCurrentPaint())
                        {
                            canvas.DrawLine(_startPoint, _endPoint, paint);
                        }
                        break;
                    case Tool.Select:
                        // Draw selection rectangle
                        using (var paint = new SKPaint
                        {
                            Color = SKColors.Blue,
                            Style = SKPaintStyle.Stroke,
                            StrokeWidth = 1,
                            PathEffect = SKPathEffect.CreateDash(new float[] { 5, 5 }, 0)
                        })
                        {
                            var rect = GetSelectionRect(_startPoint, _endPoint);
                            canvas.DrawRect(rect, paint);
                        }
                        break;
                }
            }
        }

        private SKRect GetSelectionRect(SKPoint start, SKPoint end)
        {
            return new SKRect(
                Math.Min(start.X, end.X),
                Math.Min(start.Y, end.Y),
                Math.Max(start.X, end.X),
                Math.Max(start.Y, end.Y)
            );
        }

        private SKPaint GetCurrentPaint()
        {
            return new SKPaint
            {
                Color = _currentTool == Tool.Eraser ? SKColors.White : SKColors.Black,
                StrokeWidth = _currentTool == Tool.Eraser ? 10 : 2,
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round,
                IsAntialias = true
            };
        }

        private void DrawingSurface_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("DrawingSurface_MouseDown");

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            var pos = e.GetPosition(DrawingSurface);
            _startPoint = pos.ToSKPoint();
            _endPoint = _startPoint;

            if (_currentTool == Tool.Select)
            {
                // Check if clicking inside an existing selection
                if (_hasSelection && _selectionRect.Contains(_startPoint))
                {
                    _isMovingSelection = true;
                    _selectionOffset = new SKPoint(
                        _startPoint.X - _selectionRect.Left,
                        _startPoint.Y - _selectionRect.Top
                    );
                }
                else
                {
                    // Clear previous selection if clicking outside
                    if (_hasSelection)
                    {
                        CommitSelection();
                    }
                    _isDrawing = true;
                }
            }
            else
            {
                // Clear selection when using other tools
                if (_hasSelection)
                {
                    CommitSelection();
                }
                _isDrawing = true;
            }
        }

        private void DrawingSurface_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(DrawingSurface);
            var currentPoint = pos.ToSKPoint();

            if (_isMovingSelection)
            {
                // Update selection rectangle position
                float newLeft = currentPoint.X - _selectionOffset.X;
                float newTop = currentPoint.Y - _selectionOffset.Y;

                // Keep selection within canvas bounds
                newLeft = Math.Max(0, Math.Min(newLeft, _bitmap.Width - _selectionRect.Width));
                newTop = Math.Max(0, Math.Min(newTop, _bitmap.Height - _selectionRect.Height));

                _selectionRect = new SKRect(
                    newLeft,
                    newTop,
                    newLeft + _selectionRect.Width,
                    newTop + _selectionRect.Height
                );

                DrawingSurface.InvalidateVisual();
            }
            else if (_isDrawing)
            {
                if (_currentTool == Tool.Pencil || _currentTool == Tool.Eraser)
                {
                    using (var paint = GetCurrentPaint())
                    {
                        _canvas.DrawLine(_startPoint, currentPoint, paint);
                    }
                    _startPoint = currentPoint;
                    DrawingSurface.InvalidateVisual();
                }
                else if (_currentTool == Tool.Line || _currentTool == Tool.Select)
                {
                    _endPoint = currentPoint;
                    DrawingSurface.InvalidateVisual();
                }
            }
        }

        private void DrawingSurface_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("DrawingSurface_MouseUp");

            if (e.LeftButton != MouseButtonState.Released)
            {
                return;
            }

            if (_isMovingSelection)
            {
                _isMovingSelection = false;
                DrawingSurface.InvalidateVisual();
            }
            else if (_isDrawing)
            {
                if (_currentTool == Tool.Line)
                {
                    // Commit the line to the main bitmap
                    using (var paint = GetCurrentPaint())
                    {
                        _canvas.DrawLine(_startPoint, _endPoint, paint);
                    }
                }
                else if (_currentTool == Tool.Select)
                {
                    // Create selection
                    _selectionRect = GetSelectionRect(_startPoint, _endPoint);

                    // Only create selection if it has area
                    if (_selectionRect.Width > 1 && _selectionRect.Height > 1)
                    {
                        CreateSelection();
                    }
                }

                _isDrawing = false;
                DrawingSurface.InvalidateVisual();
            }
        }

        private void CreateSelection()
        {
            // Make sure rect is in bounds
            _selectionRect = new SKRect(
                Math.Max(0, _selectionRect.Left),
                Math.Max(0, _selectionRect.Top),
                Math.Min(_bitmap.Width, _selectionRect.Right),
                Math.Min(_bitmap.Height, _selectionRect.Bottom)
            );

            // Create bitmap for selection
            _selectionBitmap = new SKBitmap(
                (int)_selectionRect.Width,
                (int)_selectionRect.Height
            );

            // Copy the area from the main bitmap
            using (var canvas = new SKCanvas(_selectionBitmap))
            {
                canvas.Clear(SKColors.Transparent);
                canvas.DrawBitmap(
                    _bitmap,
                    new SKRect(_selectionRect.Left, _selectionRect.Top, _selectionRect.Right, _selectionRect.Bottom),
                    new SKRect(0, 0, _selectionRect.Width, _selectionRect.Height)
                );
            }

            // Clear the selected area on the main bitmap (make it white)
            using (var paint = new SKPaint { Color = SKColors.White })
            {
                _canvas.DrawRect(_selectionRect, paint);
            }

            _hasSelection = true;
            DrawingSurface.InvalidateVisual();
        }

        private void CommitSelection()
        {
            if (!_hasSelection || _selectionBitmap == null) return;

            // Draw the selection bitmap onto the main bitmap at its current position
            _canvas.DrawBitmap(_selectionBitmap, _selectionRect.Left, _selectionRect.Top);

            // Clean up
            _selectionBitmap.Dispose();
            _selectionBitmap = null;
            _hasSelection = false;

            DrawingSurface.InvalidateVisual();
        }

        private void MenuItemSaveAs_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MenuItemSaveAs_Click");

            // Commit any active selection before saving
            if (_hasSelection)
            {
                CommitSelection();
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                Title = "Save as PNG",
                DefaultExt = ".png"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                SaveToPng(saveFileDialog.FileName);
            }
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MenuItemOpen_Click");

            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "PNG Image|*.png",
                Title = "Open image",
            };

            if (openFileDialog.ShowDialog() == true)
            {
                OpenPng(openFileDialog.FileName);
            }
        }

        private void OpenPng(string filePath)
        {
            // Clear any active selection
            if (_hasSelection)
            {
                CommitSelection();
            }

            using var stream = File.OpenRead(filePath);
            using var bitmap = SKBitmap.Decode(stream);
            _bitmap = new SKBitmap(
                (int)DrawingSurface.ActualWidth,
                (int)DrawingSurface.ActualHeight,
                SKColorType.Rgba8888,
                SKAlphaType.Premul
            );

            _canvas = new SKCanvas(_bitmap);

            _canvas.Clear(SKColors.White);

            _canvas.DrawBitmap(bitmap, 0, 0);

            DrawingSurface.InvalidateVisual();

            WindowTitle = filePath;
        }

        private void SaveToPng(string filePath)
        {
            if (_bitmap == null) return;

            using (var image = SKImage.FromBitmap(_bitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite(filePath))
            {
                data.SaveTo(stream);
            }
        }

        private void ButtonPencil_Click(object sender, RoutedEventArgs e)
        {
            _currentTool = Tool.Pencil;
        }

        private void ButtonEraser_Click(object sender, RoutedEventArgs e)
        {
            _currentTool = Tool.Eraser;
        }

        private void ButtonLine_Click(object sender, RoutedEventArgs e)
        {
            _currentTool = Tool.Line;
        }

        private void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            _currentTool = Tool.Select;
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            // Clear any active selection
            if (_hasSelection)
            {
                _selectionBitmap?.Dispose();
                _selectionBitmap = null;
                _hasSelection = false;
            }

            _canvas.Clear(SKColors.White);
            DrawingSurface.InvalidateVisual();
        }

        // Cut selected area to clipboard
        private void MenuItemCut_Click(object sender, RoutedEventArgs e)
        {
            if (!_hasSelection) return;

            CopySelectionToClipboard();

            // Clear selected area on the main bitmap
            using (var paint = new SKPaint { Color = SKColors.White })
            {
                _canvas.DrawRect(_selectionRect, paint);
            }

            _selectionBitmap?.Dispose();
            _selectionBitmap = null;
            _hasSelection = false;

            DrawingSurface.InvalidateVisual();
        }

        // Copy selected area to clipboard
        private void MenuItemCopy_Click(object sender, RoutedEventArgs e)
        {
            if (!_hasSelection) return;

            CopySelectionToClipboard();
        }

        // Paste from clipboard
        private void MenuItemPaste_Click(object sender, RoutedEventArgs e)
        {
            // Implementation would depend on what format you want to support
            // This is a placeholder for the concept
            if (Clipboard.ContainsImage())
            {
                var image = Clipboard.GetImage();
                if (image != null)
                {
                    // Convert image to SKBitmap and create a selection
                    // This is a simplified example and would need to be adapted
                    // based on the actual implementation details
                }
            }
        }

        private void CopySelectionToClipboard()
        {
            if (!_hasSelection || _selectionBitmap == null) return;

            // Convert SKBitmap to a format that can be placed on the clipboard
            // This is a simplified example and would need to be adapted
            // based on the actual implementation details

            // Example pseudocode:
            // var bitmapSource = _selectionBitmap.ToBitmapSource();
            // Clipboard.SetImage(bitmapSource);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Handle keyboard shortcuts
            if (e.Key == Key.Escape && _hasSelection)
            {
                // Cancel selection
                _selectionBitmap?.Dispose();
                _selectionBitmap = null;
                _hasSelection = false;
                DrawingSurface.InvalidateVisual();
                e.Handled = true;
            }
            else if (e.Key == Key.Delete && _hasSelection)
            {
                // Delete selection (make white)
                using (var paint = new SKPaint { Color = SKColors.White })
                {
                    _canvas.DrawRect(_selectionRect, paint);
                }

                _selectionBitmap?.Dispose();
                _selectionBitmap = null;
                _hasSelection = false;
                DrawingSurface.InvalidateVisual();
                e.Handled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.X && _hasSelection)
                {
                    // Cut
                    MenuItemCut_Click(this, new RoutedEventArgs());
                    e.Handled = true;
                }
                else if (e.Key == Key.C && _hasSelection)
                {
                    // Copy
                    MenuItemCopy_Click(this, new RoutedEventArgs());
                    e.Handled = true;
                }
                else if (e.Key == Key.V)
                {
                    // Paste
                    MenuItemPaste_Click(this, new RoutedEventArgs());
                    e.Handled = true;
                }
            }
        }
    }

    public static class PointExtensions
    {
        public static SKPoint ToSKPoint(this Point point)
        {
            return new SKPoint((float)point.X, (float)point.Y);
        }
    }
}