using System.Numerics;
using Raylib_cs;

namespace ded;

public class Camera
{
    private Camera2D _camera;

    public Camera()
    {
        _camera = new Camera2D
        {
            Target = Vector2.Zero,
            Offset = new Vector2(ScreenWidth / 2, ScreenHeight / 2),
            Rotation = 0,
            Zoom = 1.0f,
        };
    }
}