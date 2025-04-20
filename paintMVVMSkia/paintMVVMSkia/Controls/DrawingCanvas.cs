using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using SkiaSharp;
using Avalonia.Input;

namespace paintMVVMSkia.Controls
{
    public class DrawingCanvas : Control
    {
        private SKSurface? _surface;
        private bool _isDrawing;
        private SKPoint _lastPoint;

        static DrawingCanvas()
        {
            AffectsRender<DrawingCanvas>(BoundsProperty);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            _isDrawing = true;
            var point = e.GetPosition(this);
            _lastPoint = new SKPoint((float)point.X, (float)point.Y);
            InvalidateVisual();
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);
            if (!_isDrawing) return;

            var point = e.GetPosition(this);
            var currentPoint = new SKPoint((float)point.X, (float)point.Y);

            using (var canvas = _surface?.Canvas)
            {
                if (canvas == null) return;

                using (var paint = new SKPaint
                {
                    Color = SKColors.Black,
                    StrokeWidth = 5,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    StrokeCap = SKStrokeCap.Round
                })
                {
                    canvas.DrawLine(_lastPoint, currentPoint, paint);
                }
            }

            _lastPoint = currentPoint;
            InvalidateVisual();
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            _isDrawing = false;
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (_surface == null || 
                _surface.Canvas.LocalClipBounds.Width != Bounds.Width ||
                _surface.Canvas.LocalClipBounds.Height != Bounds.Height)
            {
                _surface?.Dispose();
                _surface = SKSurface.Create(new SKImageInfo(
                    (int)Bounds.Width, 
                    (int)Bounds.Height, 
                    SKColorType.Rgba8888, 
                    SKAlphaType.Premul));
                
                // Clear with white background
                _surface.Canvas.Clear(SKColors.White);
            }

            using (var skiaImage = _surface.Snapshot())
            using (var avaloniaImage = new DrawingImage(skiaImage))
            {
                context.DrawImage(avaloniaImage, new Rect(0, 0, Bounds.Width, Bounds.Height));
            }
        }
    }
}