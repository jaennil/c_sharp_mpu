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
    private bool _isSelecting;
    private bool _isMoving;
    private SKPoint _startPoint;
    private SKPoint _endPoint;
    private SKPoint _moveOffset;
    private SKRect _selectionRect;
    private SKBitmap _selectionBitmap;
    private SKBitmap _backgroundBitmap;
    private bool _hasSelection => _selectionBitmap != null;
    private DrawingDocument _drawingDocument;

    public override string Name => "Selection";

    public SelectionTool(ToolSettings settings, DrawingDocument document) : base(settings)
    {
        _drawingDocument = document;
    }

    public override void OnMouseDown(DrawingDocument document, SKPoint point, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            _startPoint = point;
            _endPoint = point;

            if (_hasSelection && _selectionRect.Contains(point))
            {
                _isMoving = true;
                _moveOffset = new SKPoint(point.X - _selectionRect.Left, point.Y - _selectionRect.Top);
            }
            else
            {
                CommitSelection();
                _isSelecting = true;
            }
        }
    }

    public override void OnMouseMove(DrawingDocument document, SKPoint point, MouseEventArgs e)
    {
        if (_isMoving)
        {
            float newLeft = point.X - _moveOffset.X;
            float newTop = point.Y - _moveOffset.Y;

            newLeft = Math.Max(0, Math.Min(newLeft, document.Width - _selectionRect.Width));
            newTop = Math.Max(0, Math.Min(newTop, document.Height - _selectionRect.Height));

            _selectionRect = new SKRect(
                newLeft,
                newTop,
                newLeft + _selectionRect.Width,
                newTop + _selectionRect.Height
            );
        }
        else if (_isSelecting)
        {
            _endPoint = point;
        }
    }

    public override void OnMouseUp(DrawingDocument document, SKPoint point, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Released)
        {
            if (_isMoving)
            {
                _isMoving = false;
            }
            else if (_isSelecting)
            {
                _isSelecting = false;
                _selectionRect = GetSelectionRect(_startPoint, _endPoint);

                if (_selectionRect.Width > 2 && _selectionRect.Height > 2)
                {
                    CreateSelection(document);
                }
            }
        }
    }

    public override void OnRender(SKCanvas canvas)
    {
        if (_isSelecting)
        {
            var rect = GetSelectionRect(_startPoint, _endPoint);
            using var paint = new SKPaint
            {
                Color = SKColors.Blue,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                PathEffect = SKPathEffect.CreateDash(new float[] { 5, 5 }, 0)
            };
            canvas.DrawRect(rect, paint);
        }
        else if (_hasSelection)
        {
            using (var paint = new SKPaint { Color = SKColors.White })
            {
                canvas.DrawRect(_selectionRect, paint);
            }
            canvas.DrawBitmap(_selectionBitmap, _selectionRect.Left, _selectionRect.Top);
            using (var paint = new SKPaint
            {
                Color = SKColors.Blue,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                PathEffect = SKPathEffect.CreateDash(new float[] { 5, 5 }, 0)
            })
            {
                canvas.DrawRect(_selectionRect, paint);
            }
        }
    }

    private void CreateSelection(DrawingDocument document)
    {
        _selectionRect = new SKRect(
            Math.Max(0, _selectionRect.Left),
            Math.Max(0, _selectionRect.Top),
            Math.Min(document.Width, _selectionRect.Right),
            Math.Min(document.Height, _selectionRect.Bottom)
        );

        _selectionBitmap = new SKBitmap(
            (int)_selectionRect.Width,
            (int)_selectionRect.Height
        );

        _backgroundBitmap = new SKBitmap(
            (int)_selectionRect.Width,
            (int)_selectionRect.Height
        );

        using (var canvas = new SKCanvas(_selectionBitmap))
        {
            canvas.DrawBitmap(
                document.Bitmap,
                new SKRect(_selectionRect.Left, _selectionRect.Top, _selectionRect.Right, _selectionRect.Bottom),
                new SKRect(0, 0, _selectionRect.Width, _selectionRect.Height)
            );
        }

        using (var canvas = new SKCanvas(_backgroundBitmap))
        {
            canvas.DrawBitmap(
                document.Bitmap,
                new SKRect(_selectionRect.Left, _selectionRect.Top, _selectionRect.Right, _selectionRect.Bottom),
                new SKRect(0, 0, _selectionRect.Width, _selectionRect.Height)
            );
        }

        using var paint = new SKPaint { Color = SKColors.White };
        document.Canvas.DrawRect(_selectionRect, paint);

    }

    private void CommitSelection()
    {
        if (_hasSelection == false) return;

        using var paint = new SKPaint();
        _drawingDocument.Canvas.DrawBitmap(_selectionBitmap, _selectionRect.Left, _selectionRect.Top, paint);
        CleanupSelection();
    }

    private void CleanupSelection()
    {
        _selectionBitmap?.Dispose();
        _selectionBitmap = null;
        _backgroundBitmap?.Dispose();
        _backgroundBitmap = null;
    }

    private void CancelSelection()
    {
        if (_hasSelection == false) return;

        using var paint = new SKPaint();

        _drawingDocument.Canvas.DrawBitmap(_backgroundBitmap, _selectionRect.Left, _selectionRect.Top, paint);

        CleanupSelection();
    }

    private void DeleteSelection()
    {
        if (_hasSelection == false) return;

        CleanupSelection();
    }

    private SKRect GetSelectionRect(SKPoint start, SKPoint end)
    {
        return new SKRect(
            Math.Min(start.X, end.X),
            Math.Min(start.Y, end.Y),
            Math.Max(start.X, end.X),
            Math.Max(start.Y, end.Y)
        );
    }

    public override void OnDeactivated()
    {
        base.OnDeactivated();
        CommitSelection();
    }
}
