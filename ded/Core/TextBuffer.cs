using System.Numerics;

namespace ded.Core;

public class TextBuffer
{
    private string _text = "";
    private Cursor _cursor;
    
    public string Text => _text;
    
    public TextBuffer()
    {
    }

    public void Append(char c)
    {
        _text += c;
    }

    public void RemoveLast()
    {
        _text = _text.Substring(_text.Length - 1);
    }
}
