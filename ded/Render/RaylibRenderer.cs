using System.Numerics;
using ded.Core;
using Raylib_cs;

namespace ded.Render;

public class RaylibRenderer : IRenderer
{
    public void Init()
    {
        Raylib.InitWindow(2000, 1200, "ded");
        Raylib.SetTargetFPS(144);
    }
    
    public void DrawText(TextBuffer buffer)
    {
        Raylib.DrawTextEx(buffer.Font, buffer.Text, Vector2.Zero, TextBuffer.FontSize, TextBuffer.FontSpacing, buffer.Color);
    }

    private void DrawCursor()
    {
        throw new NotImplementedException();
    }

    private void Draw()
    {
        DrawCursor();
    }

    public void Run()
    {
        while (!Raylib.WindowShouldClose())
        {
            Draw();
        }
    }
}