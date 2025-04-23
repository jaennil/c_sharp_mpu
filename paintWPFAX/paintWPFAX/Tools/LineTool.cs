using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using paintWPFAX.Models;
using SkiaSharp;

namespace paintWPFAX.Tools;

public class LineTool : ToolBase
{
    private bool _isDrawing;
    private SKPoint _startPoint;
    private SKPoint _currentPoint;
    public LineTool(ToolSettings settings) : base(settings)
    {
    }

    public override string Name => "Line";

    public override void OnMouseDown(DrawingDocument document, SKPoint point, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            _isDrawing = true;
            _startPoint = point;
            _currentPoint = point;
        }
    }

    public override void OnMouseMove(DrawingDocument document, SKPoint point, MouseEventArgs e)
    {
        if (!_isDrawing) return;

        _currentPoint = point;

        //using (var paint = GetPaint())
        //{
        //    document.Canvas.DrawLine(_startPoint, point, paint);
        //}

    }

    public override void OnMouseUp(DrawingDocument document, SKPoint point, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Released)
        {
            using var paint = GetPaint();
            document.Canvas.DrawLine(_startPoint, point, paint);
            _isDrawing = false;
        }
    }

    public override void OnRender(SKCanvas canvas)
    {
        if (!_isDrawing) return;

        using var paint = GetPaint();
        canvas.DrawLine(_startPoint, _currentPoint, paint);
    }
}
