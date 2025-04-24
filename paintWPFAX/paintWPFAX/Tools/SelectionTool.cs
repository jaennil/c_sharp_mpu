using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using paintWPFAX.Models;
using SkiaSharp;

namespace paintWPFAX.Tools;

public class SelectionTool : ToolBase
{
    private bool _isDrawing;
    private SKPoint _startPoint;
    private SKPoint _endPoint;

    public override string Name => "Selection";

    public SelectionTool(ToolSettings settings) : base(settings)
    {
        //settings.PathEffect = new SKPathEffec
    }

    public override void OnMouseDown(DrawingDocument document, SKPoint point, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            _isDrawing = true;
            _startPoint = point;
            _endPoint = point;
        }
    }

    public override void OnMouseMove(DrawingDocument document, SKPoint point, MouseEventArgs e)
    {
        if (_isDrawing == false) return;

        _endPoint = point;
    }

    public override void OnMouseUp(DrawingDocument document, SKPoint point, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Released)
        {
            using var paint = GetPaint();
            var rect = new SKRect(_startPoint.X, _startPoint.Y, point.X, point.Y);
            document.Canvas.DrawRect(rect, paint);
            _isDrawing = false;
        }
    }

    public override void OnRender(SKCanvas canvas)
    {
        if (_isDrawing == false) return;

        using var paint = GetPaint();
        var rect = new SKRect(_startPoint.X, _startPoint.Y, _endPoint.X, _endPoint.Y);
        canvas.DrawRect(rect, paint);

    }
}
