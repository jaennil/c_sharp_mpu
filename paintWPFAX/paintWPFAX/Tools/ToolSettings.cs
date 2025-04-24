using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace paintWPFAX.Tools;

public class ToolSettings
{
    public SKColor StrokeColor { get; set; } = SKColors.Black;
    public SKColor FillColor { get; set; } = SKColors.White;
    public float StrokeWidth { get; set; } = 1f;
    public bool IsAntialias { get; set; } = false;
    public SKPaintStyle PaintStyle { get; set; } = SKPaintStyle.Stroke;
    public SKStrokeCap StrokeCap { get; set; } = SKStrokeCap.Round;
    public SKPathEffect PathEffect { get; set; }

    public SKPaint CreatePaint()
    {
        return new SKPaint
        {
            Color = StrokeColor,
            StrokeWidth = StrokeWidth,
            Style = PaintStyle,
            StrokeCap = StrokeCap,
            IsAntialias = IsAntialias,
            PathEffect = PathEffect
        };
    }
}
