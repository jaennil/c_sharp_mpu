using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using paintWPFAX.Models;
using SkiaSharp;

namespace paintWPFAX.Tools;

public abstract class ToolBase : ITool
{
    protected ToolSettings Settings { get; }
    public abstract string Name { get; }
    
    protected ToolBase(ToolSettings settings)
    {
        Settings = settings;
    }

    public abstract void OnMouseDown(DrawingDocument document, SKPoint point, MouseButtonEventArgs e);
    public abstract void OnMouseMove(DrawingDocument document, SKPoint point, MouseEventArgs e);
    public abstract void OnMouseUp(DrawingDocument document, SKPoint point, MouseButtonEventArgs e);
    public virtual void OnRender(SKCanvas canvas) { }
    protected SKPaint GetPaint()
    {
        return Settings.CreatePaint();
    }

}
