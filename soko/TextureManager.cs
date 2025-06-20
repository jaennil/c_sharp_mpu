using static Raylib_cs.Raylib;
using System.Numerics;
using Raylib_cs;

namespace Game;

public static class TextureManager
{

    private static Dictionary<FloorObject, Texture2D> FloorTextures { get; }
    private static Dictionary<SurfaceObject, Texture2D> SurfaceTextures { get; }

    static TextureManager()
    {
        FloorTextures = new()
        {
            [FloorObject.Floor] = LoadTexture("resources/textures/floor.png"),
            [FloorObject.Button] = LoadTexture("resources/textures/button.png"),
            [FloorObject.Goal] = LoadTexture("resources/textures/goal.png")
        };
        foreach (var texture in FloorTextures.Values)
        {
            SetTextureFilter(texture, TextureFilter.Point);
            // SetTextureWrap();
        }

        SurfaceTextures = new()
        {
            [SurfaceObject.Wall] = LoadTexture("resources/textures/wall.png"),
            [SurfaceObject.Box] = LoadTexture("resources/textures/box.png"),
            [SurfaceObject.Player] = LoadTexture("resources/textures/player.png")
        };
        foreach (var texture in SurfaceTextures.Values)
        {
            SetTextureFilter(texture, TextureFilter.Point);
            // SetTextureWrap();
        }
    }

    public static void UnloadTextures()
    {
        foreach (var texture in FloorTextures.Values)
            UnloadTexture(texture);

        foreach (var texture in SurfaceTextures.Values)
            UnloadTexture(texture);
    }

    public static void DrawInRectangle(FloorObject floorObject, Rectangle destRectangle)
    {
        DrawInRectangle(FloorTextures[floorObject], destRectangle);
    }

    public static void DrawInRectangle(SurfaceObject surfaceObject, Rectangle destRectangle)
    {
        DrawInRectangle(SurfaceTextures[surfaceObject], destRectangle);
    }

    private static void DrawInRectangle(Texture2D texture, Rectangle destRectangle)
    {
        DrawTexturePro(texture,
                       GetFullSourceRectangle(texture),
                       destRectangle,
                       new Vector2(0, 0),
                       0,
                       Color.White);
    }

    private static Rectangle GetFullSourceRectangle(Texture2D texture)
    {
        return new Rectangle(0, 0, texture.Width, texture.Height);
    }
}
