using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FastImageViewer
{
    public partial class ImageViewerWindow : Window
    {
        private string _currentImagePath;
        private List<string> _imageFiles;
        private int _currentIndex;
        private bool _isFullScreen = false;
        private WindowState _previousWindowState;
        private WindowStyle _previousWindowStyle;
        private bool _isDragging = false;
        private Point _lastPosition;

        private List<string> _supportedExtensions = new List<string>
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".webp"
        };

        public ImageViewerWindow(string imagePath)
        {
            InitializeComponent();
            _currentImagePath = imagePath;
            LoadImageList();
            LoadCurrentImage();

            KeyDown += ImageViewerWindow_KeyDown;
        }

        private void LoadImageList()
        {
            var directory = Path.GetDirectoryName(_currentImagePath);
            if (Directory.Exists(directory))
            {
                _imageFiles = Directory.GetFiles(directory)
                    .Where(f => _supportedExtensions.Contains(Path.GetExtension(f).ToLower()))
                    .OrderBy(f => f)
                    .ToList();

                _currentIndex = _imageFiles.IndexOf(_currentImagePath);
            }
            else
            {
                _imageFiles = new List<string> { _currentImagePath };
                _currentIndex = 0;
            }
        }

        private void LoadCurrentImage()
        {
            try
            {
                if (_currentIndex >= 0 && _currentIndex < _imageFiles.Count)
                {
                    _currentImagePath = _imageFiles[_currentIndex];

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(_currentImagePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    MainImage.Source = bitmap;

                    // Обновляем информацию об изображении
                    UpdateImageInfo();

                    // Обновляем заголовок окна
                    Title = $"Просмотр изображения - {Path.GetFileName(_currentImagePath)} ({_currentIndex + 1}/{_imageFiles.Count})";

                    // Подгоняем изображение под размер окна
                    FitImageToWindow();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateImageInfo()
        {
            try
            {
                var fileInfo = new FileInfo(_currentImagePath);
                var bitmap = MainImage.Source as BitmapImage;

                var info = $"Файл: {Path.GetFileName(_currentImagePath)}\n" +
                          $"Путь: {_currentImagePath}\n" +
                          $"Размер файла: {FormatFileSize(fileInfo.Length)}\n" +
                          $"Изменен: {fileInfo.LastWriteTime:dd.MM.yyyy HH:mm:ss}";

                if (bitmap != null)
                {
                    info += $"\nРазмер изображения: {bitmap.PixelWidth} x {bitmap.PixelHeight}";
                    // info += $"\nМасштаб: {ImageScrollViewer.ZoomFactor:P0}";
                }

                TxtImageInfo.Text = info;
            }
            catch (Exception ex)
            {
                TxtImageInfo.Text = $"Ошибка получения информации: {ex.Message}";
            }
        }

        private string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} Б";
            if (bytes < 1024 * 1024) return $"{bytes / 1024:F1} КБ";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024 * 1024):F1} МБ";
            return $"{bytes / (1024 * 1024 * 1024):F1} ГБ";
        }

        private void FitImageToWindow()
        {
            if (MainImage.Source != null)
            {
                var bitmap = MainImage.Source as BitmapImage;
                if (bitmap != null)
                {
                    var scaleX = ImageScrollViewer.ActualWidth / bitmap.PixelWidth;
                    var scaleY = ImageScrollViewer.ActualHeight / bitmap.PixelHeight;
                    var scale = Math.Min(scaleX, scaleY);

                    // Не увеличиваем изображение больше оригинального размера
                    if (scale > 1.0) scale = 1.0;

                    // ImageScrollViewer.ZoomToFactor(scale);
                }
            }
        }

        private void ImageViewerWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    if (_isFullScreen)
                        ToggleFullScreen();
                    else
                        Close();
                    break;
                case Key.F11:
                    ToggleFullScreen();
                    break;
                case Key.Left:
                    ShowPreviousImage();
                    break;
                case Key.Right:
                    ShowNextImage();
                    break;
                case Key.Add:
                case Key.OemPlus:
                    ZoomIn();
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    ZoomOut();
                    break;
                case Key.D0:
                    ResetZoom();
                    break;
                case Key.Space:
                    FitImageToWindow();
                    break;
            }
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e) => ShowPreviousImage();
        private void BtnNext_Click(object sender, RoutedEventArgs e) => ShowNextImage();
        private void BtnZoomIn_Click(object sender, RoutedEventArgs e) => ZoomIn();
        private void BtnZoomOut_Click(object sender, RoutedEventArgs e) => ZoomOut();
        private void BtnResetZoom_Click(object sender, RoutedEventArgs e) => ResetZoom();
        private void BtnFitToWindow_Click(object sender, RoutedEventArgs e) => FitImageToWindow();
        private void BtnFullScreen_Click(object sender, RoutedEventArgs e) => ToggleFullScreen();
        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

        private void ShowPreviousImage()
        {
            if (_imageFiles.Count > 1)
            {
                _currentIndex = (_currentIndex - 1 + _imageFiles.Count) % _imageFiles.Count;
                LoadCurrentImage();
            }
        }

        private void ShowNextImage()
        {
            if (_imageFiles.Count > 1)
            {
                _currentIndex = (_currentIndex + 1) % _imageFiles.Count;
                LoadCurrentImage();
            }
        }

        private void ZoomIn()
        {
            // var newZoom = ImageScrollViewer.ZoomFactor * 1.2;
            // if (newZoom <= 10.0) // Максимальный зум 1000%
            {
                // ImageScrollViewer.ZoomToFactor(newZoom);
                UpdateImageInfo();
            }
        }

        private void ZoomOut()
        {
            // var newZoom = ImageScrollViewer.ZoomFactor / 1.2;
            // if (newZoom >= 0.1) // Минимальный зум 10%
            {
                // ImageScrollViewer.ZoomToFactor(newZoom);
                UpdateImageInfo();
            }
        }

        private void ResetZoom()
        {
            // ImageScrollViewer.ZoomToFactor(1.0);
            UpdateImageInfo();
        }

        private void ToggleFullScreen()
        {
            if (_isFullScreen)
            {
                // Выходим из полноэкранного режима
                WindowState = _previousWindowState;
                WindowStyle = _previousWindowStyle;
                ToolbarPanel.Visibility = Visibility.Visible;
                InfoPanel.Visibility = Visibility.Visible;
                _isFullScreen = false;
            }
            else
            {
                // Входим в полноэкранный режим
                _previousWindowState = WindowState;
                _previousWindowStyle = WindowStyle;
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                ToolbarPanel.Visibility = Visibility.Collapsed;
                InfoPanel.Visibility = Visibility.Collapsed;
                _isFullScreen = true;
            }
        }

        private void MainImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // Двойной клик - переключение полноэкранного режима
                ToggleFullScreen();
            }
            else
            {
                // Начинаем перетаскивание
                _isDragging = true;
                _lastPosition = e.GetPosition(ImageScrollViewer);
                MainImage.CaptureMouse();
            }
        }

        private void MainImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                var currentPosition = e.GetPosition(ImageScrollViewer);
                var deltaX = currentPosition.X - _lastPosition.X;
                var deltaY = currentPosition.Y - _lastPosition.Y;

                ImageScrollViewer.ScrollToHorizontalOffset(ImageScrollViewer.HorizontalOffset - deltaX);
                ImageScrollViewer.ScrollToVerticalOffset(ImageScrollViewer.VerticalOffset - deltaY);

                _lastPosition = currentPosition;
            }
        }

        private void MainImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            MainImage.ReleaseMouseCapture();
        }

        private void MainImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                // Ctrl + колесо мыши = зум
                if (e.Delta > 0)
                    ZoomIn();
                else
                    ZoomOut();
            }
            else
            {
                // Обычное колесо мыши = прокрутка
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    // Shift + колесо = горизонтальная прокрутка
                    ImageScrollViewer.ScrollToHorizontalOffset(
                        ImageScrollViewer.HorizontalOffset - e.Delta);
                }
                else
                {
                    // Вертикальная прокрутка
                    ImageScrollViewer.ScrollToVerticalOffset(
                        ImageScrollViewer.VerticalOffset - e.Delta);
                }
            }
        }
    }
}
