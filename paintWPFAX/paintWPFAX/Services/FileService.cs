using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using paintWPFAX.Models;
using SkiaSharp;

namespace paintWPFAX.Services;

public class FileService
{
    public async Task<DrawingDocument> OpenDocumentAsync(string filePath, int canvasWidth, int canvasHeight)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            return null;
        }

        using var stream = File.OpenRead(filePath);
        using var loadedBitmap = SKBitmap.Decode(stream);
        var document = new DrawingDocument(canvasWidth, canvasHeight);
        document.Canvas.Clear(SKColors.White);
        document.Canvas.DrawBitmap(loadedBitmap, 0, 0);
        document.SetFilePath(filePath);
        return document;
    }

    public async Task SaveDocumentAsync(DrawingDocument document, string filePath)
    {
        using var image = SKImage.FromBitmap(document.Bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.OpenWrite(filePath);
        document.SetFilePath(filePath);
        data.SaveTo(stream);
    }
}
