using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
 using Microsoft.Win32;

namespace FastImageViewer
{
    public partial class MainWindow : Window
    {
         private ImageCacheService _cacheService;
         private string _currentFolder = "";
        private List<string> _supportedExtensions = new List<string>
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".webp"
        };
        
        public MainWindow()
        {
             InitializeComponent();
             _cacheService = new ImageCacheService();
             LoadSettings();
        }

        private void LoadSettings()
        {
             var settings = SettingsManager.Load();
             _cacheService.ThumbnailWidth = settings.ThumbnailWidth;
             _cacheService.ThumbnailHeight = settings.ThumbnailHeight;
        }

        private async void BtnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
             var dialog = new OpenFolderDialog
            {
                 Title = "Выберите папку с изображениями",
            }
            ;
             if (dialog.ShowDialog() == true)
            {
                 _currentFolder = dialog.FolderName;
                 await OpenFolder(_currentFolder);
            }
        }

        private async void BtnGoUp_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentFolder))
            {
                var parentDir = Directory.GetParent(_currentFolder);
                if (parentDir != null)
                {
                    await OpenFolder(parentDir.FullName);
                }
            }
        }

        private async Task OpenFolder(string folderPath)
        {
             _currentFolder = folderPath;
             TxtCurrentPath.Text = _currentFolder;
             await LoadImagesFromFolder();
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentFolder))
            {
                 await LoadImagesFromFolder();
            }
        }

        private async void BtnClearCache_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите очистить весь кеш?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
             if (result == MessageBoxResult.Yes)
            {
                 SetStatus("Очистка кеша...");
                 await Task.Run(() => _cacheService.ClearCache());
                SetStatus("Кеш очищен");
                UpdateCacheInfo();
            }
        }

        private async void BtnBuildCache_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFolder))
            {
                MessageBox.Show("Сначала выберите папку", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

             await BuildCacheForCurrentFolder();
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
             var settingsWindow = new SettingsWindow();
             if (settingsWindow.ShowDialog() == true)
            {
                 LoadSettings();
                 if (!string.IsNullOrEmpty(_currentFolder))
                {
                     Task.Run(() => LoadImagesFromFolder());
                }
            }
        }

        private async Task LoadImagesFromFolder()
        {
            if (string.IsNullOrEmpty(_currentFolder) || !Directory.Exists(_currentFolder))
                 return;
             SetStatus("Загрузка изображений...");
            ShowProgress(true);
            ImagePanel.Children.Clear();

            try
            {
                var directories = Directory.GetDirectories(_currentFolder);
                var imageFiles = Directory.GetFiles(_currentFolder)
                    .Where(f => _supportedExtensions.Contains(Path.GetExtension(f).ToLower()))
                    .ToList();
                 TxtImageCount.Text = $"Папок: {directories.Length}, Изображений: {imageFiles.Count}";

                var totalItems = directories.Length + imageFiles.Count;
                int processedItems = 0;

                foreach (var dirPath in directories)
                {
                    processedItems++;
                     UpdateProgress($"Загрузка папки {processedItems} из {totalItems}", processedItems * 100 / totalItems);
                    var folderControl = await CreateFolderControl(dirPath);
                    if (folderControl != null)
                    {
                         Dispatcher.Invoke(() => ImagePanel.Children.Add(folderControl));
                    }
                }

                for (int i = 0; i < imageFiles.Count; i++)
                {
                     var filePath = imageFiles[i];
                    processedItems++;
                     UpdateProgress($"Загрузка {processedItems} из {totalItems}", processedItems * 100 / totalItems);
                     var imageControl = await CreateImageControl(filePath);
                    if (imageControl != null)
                    {
                         Dispatcher.Invoke(() => ImagePanel.Children.Add(imageControl));
                    }
                }

                 UpdateCacheInfo();
                 SetStatus($"Загружено {totalItems} элементов");
            }
            catch (Exception ex)
            {
                 SetStatus($"Ошибка: {ex.Message}");
            }
            finally
            {
                 ShowProgress(false);
            }
        }

        private async Task<Border> CreateFolderControl(string folderPath)
        {
            try
            {
                var folderPreviewImages = new List<System.Windows.Controls.Image>();
                var firstFourImages = Directory.GetFiles(folderPath)
                                               .Where(f => _supportedExtensions.Contains(Path.GetExtension(f).ToLower()))
                                               .Take(4)
                                               .ToList();

                double previewWidth = _cacheService.ThumbnailWidth / 2.0 - 2;
                double previewHeight = _cacheService.ThumbnailHeight / 2.0 - 2;

                foreach (var imagePath in firstFourImages)
                {
                     var thumbnail = await _cacheService.GetThumbnailAsync(imagePath);
                    if (thumbnail != null)
                    {
                        var image = new System.Windows.Controls.Image
                        {
                            Source = thumbnail,
                            Width = previewWidth,
                            Height = previewHeight,
                            Stretch = Stretch.UniformToFill,
                           
                            Margin = new Thickness(1)
                        };
                        folderPreviewImages.Add(image);
                    }
                }

                var previewGrid = new WrapPanel
                {
                    Width = _cacheService.ThumbnailWidth,
                    Height = _cacheService.ThumbnailHeight,
                    Orientation = Orientation.Horizontal
                };

                if (folderPreviewImages.Any())
                {
                    foreach (var img in folderPreviewImages)
                    {
                        previewGrid.Children.Add(img);
                    }
                }
                else
                {
                    // Отобразить иконку папки, если нет изображений
                    previewGrid.Background = new SolidColorBrush(Colors.LightGray);
                }


                var folderName = Path.GetFileName(folderPath);
                 var textBlock = new TextBlock
                {
                     Text = folderName,
                     TextAlignment = TextAlignment.Center,
                     Margin = new Thickness(5),
                     FontSize = 10,
                     TextWrapping = TextWrapping.Wrap,
                     MaxWidth = _cacheService.ThumbnailWidth
                }
                ;

                 var stackPanel = new StackPanel();
                stackPanel.Children.Add(previewGrid);
                stackPanel.Children.Add(textBlock);

                 var border = new Border
                {
                     Child = stackPanel,
                     Style = (Style)FindResource("ImageBorderStyle"),
                     ToolTip = $"Папка: {folderName}\nПуть: {folderPath}",
                     Cursor = System.Windows.Input.Cursors.Hand
                }
                ;

                border.MouseLeftButtonDown += async (s, e) => await OpenFolder(folderPath);

                 return border;
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Ошибка создания превью для папки {folderPath}: {ex.Message}");
                 return null;
            }
        }


        private async Task<Border> CreateImageControl(string filePath)
        {
            try
            {
                 var thumbnail = await _cacheService.GetThumbnailAsync(filePath);
                 if (thumbnail == null) return null; 

                 var image = new System.Windows.Controls.Image
                {
                     Source = thumbnail, 
                     Width = _cacheService.ThumbnailWidth, 
                     Height = _cacheService.ThumbnailHeight, 
                     Stretch = Stretch.Uniform, 
                     Cursor = System.Windows.Input.Cursors.Hand
                }
                ;
                 image.MouseLeftButtonDown += (s, e) => OpenImageViewer(filePath); 

                var fileName = Path.GetFileName(filePath);
                var fileInfo = new FileInfo(filePath);
                 var fileSize = FormatFileSize(fileInfo.Length); 
                 var textBlock = new TextBlock
                {
                     Text = $"{fileName}\n{fileSize}", 
                     TextAlignment = TextAlignment.Center, 
                     Margin = new Thickness(5), 
                     FontSize = 10, 
                     TextWrapping = TextWrapping.Wrap, 
                     MaxWidth = _cacheService.ThumbnailWidth
                }
                ;
                 var stackPanel = new StackPanel(); 
                stackPanel.Children.Add(image);
                stackPanel.Children.Add(textBlock);

                 var border = new Border
                {
                     Child = stackPanel, 
                     Style = (Style)FindResource("ImageBorderStyle"), 
                     ToolTip = $"Путь: {filePath}\nРазмер: {fileSize}\nИзменен: {fileInfo.LastWriteTime:dd.MM.yyyy HH:mm}"
                }
                ;

                 return border; 
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Ошибка создания превью для {filePath}: {ex.Message}"); 
                 return null; 
            }
        }

        private void OpenImageViewer(string filePath)
        {
             var imageViewer = new ImageViewerWindow(filePath); 
             imageViewer.Show(); 
        }

        private async Task BuildCacheForCurrentFolder()
        {
            if (string.IsNullOrEmpty(_currentFolder))
                 return; 
             SetStatus("Построение кеша..."); 
            ShowProgress(true);

            try
            {
                var imageFiles = Directory.GetFiles(_currentFolder)
                    .Where(f => _supportedExtensions.Contains(Path.GetExtension(f).ToLower()))
                    .ToList(); 
                for (int i = 0; i < imageFiles.Count; i++)
                {
                     var filePath = imageFiles[i]; 
                    UpdateProgress($"Обработка {i + 1} из {imageFiles.Count}",
                        (i + 1) * 100 / imageFiles.Count); 
                     await _cacheService.GetThumbnailAsync(filePath); 
                }

                 SetStatus($"Кеш построен для {imageFiles.Count} изображений"); 
                 UpdateCacheInfo(); 
            }
            catch (Exception ex)
            {
                 SetStatus($"Ошибка построения кеша: {ex.Message}"); 
            }
            finally
            {
                 ShowProgress(false); 
            }
        }

        private void UpdateCacheInfo()
        {
             var cacheSize = _cacheService.GetCacheSize(); 
             var cacheCount = _cacheService.GetCacheCount(); 
             TxtCacheInfo.Text = $"Кеш: {cacheCount} файлов ({FormatFileSize(cacheSize)})"; 
        }

        private string FormatFileSize(long bytes)
        {
             if (bytes < 1024) return $"{bytes} Б"; 
             if (bytes < 1024 * 1024) return $"{bytes / 1024:F1} КБ"; 
             if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024 * 1024):F1} МБ"; 
             return $"{bytes / (1024 * 1024 * 1024):F1} ГБ"; 
        }

        private void SetStatus(string message)
        {
             Dispatcher.Invoke(() => StatusText.Text = message); 
        }

        private void ShowProgress(bool show)
        {
            Dispatcher.Invoke(() =>
            {
            ProgressBar.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            ProgressText.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
             if (!show) 
                {
                ProgressBar.Value = 0;
                ProgressText.Text = "";
            }
            }); 
        }

        private void UpdateProgress(string text, int value)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressText.Text = text;
                ProgressBar.Value = value;
                }); 
        }

        protected override void OnClosed(EventArgs e)
        {
             _cacheService?.Dispose(); 
             base.OnClosed(e); 
        }
    }
}