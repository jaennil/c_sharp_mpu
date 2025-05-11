using System.Numerics;
using Raylib_cs;

namespace ded.Core;

public class TextBuffer
{
    private string _text = "";
    private Cursor _cursor;
    private readonly Font _font;
    public const float FontSize = 32;
    public const float FontSpacing = 1;
    public Color Color = Color.White;
    private Vector2 _characterSize;
    
    public Font Font => _font;
    public string Text => _text;
    
    public TextBuffer()
    {
        _font = LoadFont("fonts/FireCodeNerdFont-Regular.ttf");
        _characterSize = MeasureFontCharacterSize();
        _cursor = new Cursor((int)_characterSize.X, (int)_characterSize.Y);
    }

    public void Append(char c)
    {
        _text += c;
    }

    public void RemoveLast()
    {
        _text = _text.Substring(_text.Length - 1);
    }

    private Font LoadFont(string filePath)
    {
        return Raylib.LoadFontEx(filePath, (int)FontSize, null, 0);
    }

    private Vector2 MeasureFontCharacterSize()
    {
        return Raylib.MeasureTextEx(_font, "W", FontSize, FontSpacing);
    }
}
