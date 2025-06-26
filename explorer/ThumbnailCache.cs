using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FastImageViewer;

public class ThumbnailCache
{
    private readonly string cacheFolder;
    private readonly ConcurrentDictionary<string, string> cacheIndex = new ConcurrentDictionary<string, string>();

    public ThumbnailCache(string cacheFolder)
    {
        this.cacheFolder = cacheFolder;

        // Создаем папку кеша, если ее нет
        if (!Directory.Exists(cacheFolder))
        {
            Directory.CreateDirectory(cacheFolder);
        }
        else
        {
            // Загружаем индекс существующих превью
            foreach (var file in Directory.GetFiles(cacheFolder, "*.cache"))
            {
                var originalPath = Path.GetFileNameWithoutExtension(file);
                cacheIndex[originalPath] = file;
            }
        }
    }

    public async Task<BitmapSource> GetThumbnailAsync(string imagePath, int width, int height)
    {
        // Генерируем уникальный ключ для кеша (путь + размер)
        var cacheKey = $"{imagePath}_{width}x{height}";
        var cacheFilePath = Path.Combine(cacheFolder, $"{GetSafeFileName(cacheKey)}.cache");

        // Проверяем, есть ли уже превью в кеше
        if (cacheIndex.TryGetValue(cacheKey, out var existingCacheFile) && File.Exists(existingCacheFile))
        {
            return await LoadCachedThumbnailAsync(existingCacheFile);
        }

        // Если нет в кеше - создаем новое превью
        var thumbnail = await CreateThumbnailAsync(imagePath, width, height);

        // Сохраняем в кеш
        await SaveThumbnailToCacheAsync(thumbnail, cacheFilePath);
        cacheIndex[cacheKey] = cacheFilePath;

        return thumbnail;
    }

    private async Task<BitmapSource> CreateThumbnailAsync(string imagePath, int width, int height)
    {
        return await Task.Run(() =>
        {
            try
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(imagePath);
                image.DecodePixelWidth = width;
                image.DecodePixelHeight = height;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze(); // Для безопасного использования в других потоках
                return image;
            }
            catch
            {
                // В случае ошибки возвращаем заглушку
                return CreateErrorThumbnail(width, height);
            }
        });
    }

    private BitmapSource CreateErrorThumbnail(int width, int height)
    {
        var drawingVisual = new System.Windows.Media.DrawingVisual();
        using (var drawingContext = drawingVisual.RenderOpen())
        {
            drawingContext.DrawRectangle(
                System.Windows.Media.Brushes.LightGray,
                new System.Windows.Media.Pen(System.Windows.Media.Brushes.Red, 1),
                new System.Windows.Rect(0, 0, width, height));
            drawingContext.DrawText(
                new System.Windows.Media.FormattedText("Ошибка",
                    System.Globalization.CultureInfo.CurrentCulture,
                    System.Windows.FlowDirection.LeftToRight,
                    new System.Windows.Media.Typeface("Arial"),
                    12,
                    System.Windows.Media.Brushes.Red,
                    VisualTreeHelper.GetDpi(drawingVisual).PixelsPerDip),
                new System.Windows.Point(5, height / 2 - 10));
        }

        var renderTargetBitmap = new RenderTargetBitmap(
            width, height, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
        renderTargetBitmap.Render(drawingVisual);
        renderTargetBitmap.Freeze();
        return renderTargetBitmap;
    }

    private async Task SaveThumbnailToCacheAsync(BitmapSource thumbnail, string cacheFilePath)
    {
        await Task.Run(() =>
        {
            using (var fileStream = new FileStream(cacheFilePath, FileMode.Create))
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(thumbnail));
                encoder.Save(fileStream);
            }
        });
    }

    private async Task<BitmapSource> LoadCachedThumbnailAsync(string cacheFilePath)
    {
        return await Task.Run(() =>
        {
            try
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(cacheFilePath);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();
                return image;
            }
            catch
            {
                return CreateErrorThumbnail(200, 150);
            }
        });
    }

    public async Task ClearCacheAsync()
    {
        await Task.Run(() =>
        {
            foreach (var file in Directory.GetFiles(cacheFolder))
            {
                try { File.Delete(file); }
                catch { /* Игнорируем ошибки удаления */ }
            }
            cacheIndex.Clear();
        });
    }

    public async Task RebuildCacheForFolderAsync(string folderPath, int width, int height, IProgress<int> progress = null)
    {
        var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
        var files = Directory.GetFiles(folderPath)
            .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLower()))
            .ToList();

        int totalFiles = files.Count;
        int processed = 0;

        foreach (var file in files)
        {
            var cacheKey = $"{file}_{width}x{height}";
            var cacheFilePath = Path.Combine(cacheFolder, $"{GetSafeFileName(cacheKey)}.cache");

            // Создаем новое превью и сохраняем в кеш
            var thumbnail = await CreateThumbnailAsync(file, width, height);
            await SaveThumbnailToCacheAsync(thumbnail, cacheFilePath);
            cacheIndex[cacheKey] = cacheFilePath;

            processed++;
            progress?.Report((int)((double)processed / totalFiles * 100));
        }
    }

    private string GetSafeFileName(string fileName)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(c, '_');
        }
        return fileName;
    }
}
