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
    
    public App()
    {
        _renderer = new RaylibRenderer();
        _renderer.Init();
    }

    public void Run()
    {
        _renderer.Run();
    }
}
