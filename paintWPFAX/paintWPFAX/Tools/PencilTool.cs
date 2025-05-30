﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using paintWPFAX.Models;
using SkiaSharp;

namespace paintWPFAX.Tools;

public class PencilTool : ToolBase
{
    private bool _isDrawing;
    private SKPoint _lastPoint;
    public override string Name => "Pencil";
    public PencilTool(ToolSettings settings) : base(settings)
    {
    }


    public override void OnMouseDown(DrawingDocument document, SKPoint point, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            _isDrawing = true;
            _lastPoint = point;

            using var paint = GetPaint();
            document.Canvas.DrawPoint(point, paint);
        }
    }

    public override void OnMouseMove(DrawingDocument document, SKPoint point, MouseEventArgs e)
    {
        if (!_isDrawing) return;

        using var paint = GetPaint();
        document.Canvas.DrawLine(_lastPoint, point, paint);

        _lastPoint = point;
    }

    public override void OnMouseUp(DrawingDocument document, SKPoint point, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Released)
        {
            _isDrawing = false;
        }
    }
}
