using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace FastImageViewer
{
    public partial class SettingsWindow : Window
    {
        private AppSettings _settings;
        private ImageCacheService _cacheService;

        public SettingsWindow()
        {
            InitializeComponent();
            _settings = SettingsManager.Load();
            _cacheService = new ImageCacheService();
            LoadSettings();
            UpdateCacheInfo();
        }

        private void LoadSettings()
        {
            TxtWidth.Text = _settings.ThumbnailWidth.ToString();
            TxtHeight.Text = _settings.ThumbnailHeight.ToString();
            ChkRememberFolder.IsChecked = _settings.RememberLastFolder;
            TxtMemoryCache.Text = _settings.MaxMemoryCache.ToString();
        }

        private void UpdateCacheInfo()
        {
            try
            {
                var cacheSize = _cacheService.GetCacheSize();
                var cacheCount = _cacheService.GetCacheCount();
                var cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "FastImageViewer", "Cache");

                TxtCacheInfo.Text = $"Количество файлов: {cacheCount}\n" +
                                   $"Размер на диске: {FormatFileSize(cacheSize)}\n" +
                                   $"Расположение: {cachePath}";
            }
            catch (Exception ex)
            {
                TxtCacheInfo.Text = $"Ошибка получения информации о кеше: {ex.Message}";
            }
        }

        private string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} Б";
            if (bytes < 1024 * 1024) return $"{bytes / 1024:F1} КБ";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024 * 1024):F1} МБ";
            return $"{bytes / (1024 * 1024 * 1024):F1} ГБ";
        }

        private void BtnPreset_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string tag)
            {
                var sizes = tag.Split(',');
                if (sizes.Length == 2)
                {
                    TxtWidth.Text = sizes[0];
                    TxtHeight.Text = sizes[1];
                }
            }
        }

        private void BtnOpenCacheFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "FastImageViewer", "Cache");

                if (Directory.Exists(cachePath))
                {
                    Process.Start("explorer.exe", cachePath);
                }
                else
                {
                    MessageBox.Show("Папка кеша не существует", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия папки кеша: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClearCacheSettings_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите очистить весь кеш?\n" +
                                       "",
                                       "Подтверждение",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _cacheService.ClearCache();
                    UpdateCacheInfo();
                    MessageBox.Show("Кеш успешно очищен", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка очистки кеша: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateAndSaveSettings())
            {
                DialogResult = true;
                Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateAndSaveSettings()
        {
            try
            {
                // Валидация ширины
                if (!int.TryParse(TxtWidth.Text, out int width) || width < 50 || width > 1000)
                {
                    MessageBox.Show("Ширина должна быть числом от 50 до 1000 пикселей",
                        "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtWidth.Focus();
                    return false;
                }

                // Валидация высоты
                if (!int.TryParse(TxtHeight.Text, out int height) || height < 50 || height > 1000)
                {
                    MessageBox.Show("Высота должна быть числом от 50 до 1000 пикселей",
                        "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtHeight.Focus();
                    return false;
                }

                // Валидация кеша в памяти
                if (!int.TryParse(TxtMemoryCache.Text, out int memoryCache) || memoryCache < 10 || memoryCache > 1000)
                {
                    MessageBox.Show("Количество изображений в памяти должно быть от 10 до 1000",
                        "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtMemoryCache.Focus();
                    return false;
                }

                // Сохраняем настройки
                _settings.ThumbnailWidth = width;
                _settings.ThumbnailHeight = height;
                _settings.RememberLastFolder = ChkRememberFolder.IsChecked ?? true;
                _settings.MaxMemoryCache = memoryCache;

                SettingsManager.Save(_settings);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения настроек: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _cacheService?.Dispose();
            base.OnClosed(e);
        }
    }
}
