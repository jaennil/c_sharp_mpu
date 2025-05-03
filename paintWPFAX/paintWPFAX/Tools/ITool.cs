using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using paintWPFAX.Models;
using SkiaSharp;

namespace paintWPFAX.Tools
{
    public interface ITool
    {
        string Name { get; }

        void OnActivated();
        void OnDeactivated();

        void OnMouseDown(DrawingDocument document, SKPoint point, MouseButtonEventArgs e);
        void OnMouseMove(DrawingDocument document, SKPoint point, MouseEventArgs e);
        void OnMouseUp(DrawingDocument document, SKPoint point, MouseButtonEventArgs e);
        void OnRender(SKCanvas canvas);
    }
}
