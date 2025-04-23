using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace paintWPFAX.Models;

public class DrawingDocument
{
    public SKBitmap Bitmap { get; private set; }
    public SKCanvas Canvas { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public string FilePath { get; private set; }
    public string DisplayName => string.IsNullOrEmpty(FilePath) ? "Untitiled" : System.IO.Path.GetFileName(FilePath);

    public EventHandler ContentChanged;

    public DrawingDocument(int w, int h)
    {
        Width = w;
        Height = h;
        Bitmap = new SKBitmap(Width, Height, SKColorType.Rgba8888, SKAlphaType.Premul);
        Canvas = new SKCanvas(Bitmap);
        Canvas.Clear(SKColors.White);
    }

    public void Clear()
    {
        Canvas.Clear(SKColors.White);
    }

    public void SetFilePath(string filePath)
    {
        FilePath = filePath;
        NotifyContentChanged();
    }

    private void NotifyContentChanged()
    {
        ContentChanged?.Invoke(this, EventArgs.Empty);
    }
}
