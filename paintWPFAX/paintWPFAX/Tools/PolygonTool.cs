using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using paintWPFAX.Models;
using SkiaSharp;

namespace paintWPFAX.Tools;

public class PolygonTool : ToolBase
{
    private SKPoint _startPoint;
    private SKPoint _currentPoint;

    public PolygonTool(ToolSettings settings) : base(settings)
    {
    }

    public override string Name => "Polygon";

    public override void OnMouseDown(DrawingDocument document, SKPoint point, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            _startPoint = point;
            _currentPoint = point;
            using var paint = GetPaint();
            // TODO
        }
    }

    public override void OnMouseMove(DrawingDocument document, SKPoint point, MouseEventArgs e)
    {
        _currentPoint = point;
    }

    public override void OnMouseUp(DrawingDocument document, SKPoint point, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Released)
        {
            using var paint = GetPaint();
            document.Canvas.DrawLine(_startPoint, point, paint);
        }
    }

    public override void OnRender(SKCanvas canvas)
    {
        using var paint = GetPaint();
        canvas.DrawLine(_startPoint, _currentPoint, paint);
    }
}
