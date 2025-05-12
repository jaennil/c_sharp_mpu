using System.Diagnostics;
using System.Numerics;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;

namespace ded;

public class App
{
    private const int ScreenWidth = 2000;
    private const int ScreenHeight = 1200;
    private readonly Font _font;
    private const int FontSize = 32;
    private const int FontSpacing = 1;
    private readonly Camera2D _camera;
    private string _text = "";
    private readonly Color _cursorColor = Color.White;
    private readonly Color _textColor = Color.White;
    private readonly Color _backgroundColor = Color.Black;
    private Vector2 _cursorPosition = Vector2.Zero;
    private readonly int _fontCharacterWidth;
    private bool _debug;
    private bool _coordinateAxis;
    private bool _grid;
    
    public App()
    {
        Raylib.InitWindow(ScreenWidth, ScreenHeight, "ded");
        Raylib.SetTargetFPS(144);
        
        _camera = new Camera2D
        {
            Target = Vector2.Zero,
            Offset = new Vector2(ScreenWidth / 2, ScreenHeight / 2),
            Rotation = 0,
            Zoom = 1.0f,
        };

        _font = Raylib.LoadFontEx("FiraCodeNerdFont-Regular.ttf", FontSize, null, 0); 
        
        _fontCharacterWidth = (int)Raylib.MeasureTextEx(_font, "W", FontSize, FontSpacing).X;
        
        rlImGui.Setup();
    }

    public void Run()
    {
        while (!Raylib.WindowShouldClose())
        {
            HandleInput();
            Draw();
        }

        Cleanup();
    }

    private void HandleInput()
    {
        for (var chr = Raylib.GetCharPressed(); chr > 0; chr = Raylib.GetCharPressed())
        {
            _text += (char)chr;
            _cursorPosition.X += _fontCharacterWidth + FontSpacing;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Backspace))
        {
            _text = _text[..^1];
            _cursorPosition.X -= _fontCharacterWidth + FontSpacing;
        }
        else if (Raylib.IsKeyPressed(KeyboardKey.Enter))
        {
            _text += "\n";
            
            // +2 for line spacing
            _cursorPosition = new Vector2(0, _cursorPosition.Y+FontSize+2);
        }
        else if (Raylib.IsKeyPressed(KeyboardKey.F2))
        {
            _debug = !_debug;
        }
    }

    private void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(_backgroundColor);
        Raylib.BeginMode2D(_camera);
        if (_coordinateAxis) DrawCoordinateAxis();
        DrawText();
        DrawCursor();
        Raylib.EndMode2D();
        if (_debug) DrawDebug();
        Raylib.EndDrawing();
    }

    private void DrawText()
    {
        Raylib.DrawTextEx(_font, _text, Vector2.Zero, FontSize, FontSpacing, _textColor);
    }

    private void DrawCursor()
    {
        Raylib.DrawRectangleV(_cursorPosition, new Vector2(_fontCharacterWidth, FontSize), _cursorColor);
    }

    private void DrawDebug()
    {
        rlImGui.Begin();

        if (ImGui.Begin("Debug Panel"))
        {
            ImGui.Text($"Font Character Width: {_fontCharacterWidth}");
            ImGui.Text($"Cursor Position: {_cursorPosition}");
            ImGui.Text($"Camera Offset: {_camera.Offset}");
            ImGui.Text($"Camera Zoom: {_camera.Zoom}");
            ImGui.Text($"Camera Target: {_camera.Target}");
            ImGui.Text($"Camera Rotation: {_camera.Rotation}");
            ImGui.Checkbox("Coordinate Axis", ref _coordinateAxis);
            ImGui.Checkbox("Grid", ref _grid);
        }
        ImGui.End();
        
        rlImGui.End();
    }

    private void Cleanup()
    {
        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }

    private void DrawCoordinateAxis()
    {
        Raylib.DrawLine(-10000, 0, 10000, 0, Color.Red);
        Raylib.DrawLine(0, 10000, 0, -10000, Color.Red);
    }

    private void DrawGrid()
    {
        
    }


    private void UpdateCursorPosition()
    {
        
    }
}
