using System.Numerics;
using Raylib_cs;

namespace ded;

public class Cursor
{
    private Vector2 _linePosition;
    private Vector2 _screenPosition;
    private int _width;
    private int _height;
    private Color _color = Color.White;

    public Cursor(int width, int height)
    {
        
    }

    public void Draw()
    {
        Raylib.DrawRectangleV(_screenPosition, new Vector2(_width, _height), _color);
    }
}
