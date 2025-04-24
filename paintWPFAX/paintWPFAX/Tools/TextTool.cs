using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using paintWPFAX.Models;
using SkiaSharp;

namespace paintWPFAX.Tools;

public class TextTool : ToolBase
{
    public TextTool(ToolSettings settings) : base(settings)
    {
        settings.StrokeCap = SKStrokeCap.Square;
        settings.PaintStyle = SKPaintStyle.Fill;
    }

    public override string Name => "Text";

    public override void OnMouseDown(DrawingDocument document, SKPoint point, MouseButtonEventArgs e)
    {
    }

    public override void OnMouseMove(DrawingDocument document, SKPoint point, MouseEventArgs e)
    {
    }

    public override void OnMouseUp(DrawingDocument document, SKPoint point, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Released)
        {
            using var paint = GetPaint();
            document.Canvas.DrawText("Hello", point, SKTextAlign.Center, new SKFont(SKTypeface.Default), paint);
        }
    }
}
