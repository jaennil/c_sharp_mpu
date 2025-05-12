using System.Diagnostics;
using System.Numerics;
using ded.Render;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;

namespace ded;

public class App
{
    private IRenderer _renderer;
    private InputHandler _inputHandler;
    private bool running = true;
    
    public App()
    {
        _renderer = new RaylibRenderer();
        _renderer.Init();
        _inputHandler = new InputHandler();
    }

    public void Run()
    {
        while (running)
        {
            _inputHandler.Handle();
            _renderer.Run();
        }
    }
}
