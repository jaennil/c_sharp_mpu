using System;
using System.Collections.Concurrent;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Data.Sqlite;
// Add the following directive at the top of the file to ensure the correct assembly is referenced.
using System.Drawing; // Ensure this is present

// Additionally, ensure that the System.Drawing.Common NuGet package is installed in your project.
// You can install it using the following command in the NuGet Package Manager Console:
// Install-Package System.Drawing.Common
using System.Data;

namespace FastImageViewer
{
    public class ImageCacheService : IDisposable
    {
        private readonly string _cacheDbPath;
        private readonly string _cacheFolder;
        private SqliteConnection _connection;
        private readonly ConcurrentDictionary<string, BitmapImage> _memoryCache;

        public int ThumbnailWidth { get; set; } = 150;
        public int ThumbnailHeight { get; set; } = 150;

        public ImageCacheService()
        {
            _cacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FastImageViewer", "Cache");
            Directory.CreateDirectory(_cacheFolder);

            _cacheDbPath = Path.Combine(_cacheFolder, "cache.db");
            _memoryCache = new ConcurrentDictionary<string, BitmapImage>();

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            _connection = new SqliteConnection($"Data Source={_cacheDbPath};");
            _connection.Open();

            var createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Thumbnails (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FilePath TEXT NOT NULL UNIQUE,
                    FileHash TEXT NOT NULL,
                    ThumbnailPath TEXT NOT NULL,
                    Width INTEGER NOT NULL,
                    Height INTEGER NOT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    LastModified DATETIME NOT NULL
                )";

            using (var command = new SqliteCommand(createTableQuery, _connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public async Task<BitmapImage> GetThumbnailAsync(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            var fileHash = GetFileHash(filePath);
            var fileInfo = new FileInfo(filePath);

            // Проверяем память
            if (_memoryCache.TryGetValue(filePath, out var cachedImage))
            {
                return cachedImage;
            }

            // Проверяем базу данных
            var thumbnailPath = await GetThumbnailFromDatabaseAsync(filePath, fileHash, fileInfo.LastWriteTime);

            if (!string.IsNullOrEmpty(thumbnailPath) && File.Exists(thumbnailPath))
            {
                var thumbnail = LoadBitmapImage(thumbnailPath);
                if (thumbnail != null)
                {
                    _memoryCache.TryAdd(filePath, thumbnail);
                    return thumbnail;
                }
            }

            // Создаем новое превью
            var newThumbnail = await CreateThumbnailAsync(filePath, fileHash, fileInfo.LastWriteTime);
            if (newThumbnail != null)
            {
                _memoryCache.TryAdd(filePath, newThumbnail);
            }

            return newThumbnail;
        }

        private async Task<string> GetThumbnailFromDatabaseAsync(string filePath, string fileHash, DateTime lastModified)
        {
            return await Task.Run(() =>
            {
                var query = @"
                    SELECT ThumbnailPath, FileHash, Width, Height, LastModified 
                    FROM Thumbnails 
                    WHERE FilePath = @filePath";

                using (var command = new SqliteCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@filePath", filePath);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var dbHash = reader.GetString("FileHash");
                            var dbWidth = reader.GetInt32("Width");
                            var dbHeight = reader.GetInt32("Height");
                            var dbLastModified = DateTime.Parse(reader.GetString("LastModified"));
                            var thumbnailPath = reader.GetString("ThumbnailPath");

                            // Проверяем актуальность кеша
                            if (dbHash == fileHash &&
                                dbWidth == ThumbnailWidth &&
                                dbHeight == ThumbnailHeight &&
                                dbLastModified >= lastModified)
                            {
                                return thumbnailPath;
                            }
                        }
                    }
                }
                return null;
            });
        }

        private async Task<BitmapImage> CreateThumbnailAsync(string filePath, string fileHash, DateTime lastModified)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var thumbnailFileName = $"{fileHash}_{ThumbnailWidth}x{ThumbnailHeight}.jpg";
                    var thumbnailPath = Path.Combine(_cacheFolder, thumbnailFileName);

                    // Создаем превью
                    using (var originalImage = new Bitmap(filePath))
                    {
                        var thumbnailSize = CalculateThumbnailSize(originalImage.Width, originalImage.Height);
                        using Bitmap thumbnail = new Bitmap(thumbnailSize.Width, thumbnailSize.Height);
                        using (var graphics = Graphics.FromImage(thumbnail))
                        {
                            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                            graphics.DrawImage(originalImage, 0, 0, thumbnailSize.Width, thumbnailSize.Height);
                        }

                        // Сохраняем превью
                        thumbnail.Save(thumbnailPath, ImageFormat.Jpeg);
                    }

                    // Сохраняем информацию в базу данных
                    SaveThumbnailToDatabase(filePath, fileHash, thumbnailPath, lastModified);

                    // Загружаем созданное превью
                    return LoadBitmapImage(thumbnailPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка создания превью для {filePath}: {ex.Message}");
                    return null;
                }
            });
#pragma warning restore CS8603 // Possible null reference return.
        }

        private Size CalculateThumbnailSize(int originalWidth, int originalHeight)
        {
            double ratioX = (double)ThumbnailWidth / originalWidth;
            double ratioY = (double)ThumbnailHeight / originalHeight;
            double ratio = Math.Min(ratioX, ratioY);

            return new Size(
                (int)(originalWidth * ratio),
                (int)(originalHeight * ratio)
            );
        }

        private void SaveThumbnailToDatabase(string filePath, string fileHash, string thumbnailPath, DateTime lastModified)
        {
            var query = @"
                INSERT OR REPLACE INTO Thumbnails (FilePath, FileHash, ThumbnailPath, Width, Height, LastModified)
                VALUES (@filePath, @fileHash, @thumbnailPath, @width, @height, @lastModified)";

            using (var command = new SqliteCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@filePath", filePath);
                command.Parameters.AddWithValue("@fileHash", fileHash);
                command.Parameters.AddWithValue("@thumbnailPath", thumbnailPath);
                command.Parameters.AddWithValue("@width", ThumbnailWidth);
                command.Parameters.AddWithValue("@height", ThumbnailHeight);
                command.Parameters.AddWithValue("@lastModified", lastModified.ToString("yyyy-MM-dd HH:mm:ss"));
                command.ExecuteNonQuery();
            }
        }

        private BitmapImage LoadBitmapImage(string path)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        private string GetFileHash(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                var fileInfo = new FileInfo(filePath);
                var input = $"{filePath}_{fileInfo.Length}_{fileInfo.LastWriteTime:yyyyMMddHHmmss}";
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(hash).Replace("/", "_").Replace("+", "-");
            }
        }

        public void ClearCache()
        {
            _memoryCache.Clear();

            // Очищаем файлы превью
            var files = Directory.GetFiles(_cacheFolder, "*.jpg");
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }

            // Очищаем базу данных
            using (var command = new SqliteCommand("DELETE FROM Thumbnails", _connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public long GetCacheSize()
        {
            long totalSize = 0;
            var files = Directory.GetFiles(_cacheFolder);
            foreach (var file in files)
            {
                try
                {
                    totalSize += new FileInfo(file).Length;
                }
                catch { }
            }
            return totalSize;
        }

        public int GetCacheCount()
        {
            using (var command = new SqliteCommand("SELECT COUNT(*) FROM Thumbnails", _connection))
            {
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
            _memoryCache?.Clear();
        }
    }
}
