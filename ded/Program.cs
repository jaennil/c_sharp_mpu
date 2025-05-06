using ded;

var app = new App();
app.Run();

// var cursorPosition = Vector2.Zero;
//
// while (!Raylib.WindowShouldClose())
// {
//     int key = Raylib.GetCharPressed();
//     while (key > 0)
//     {
//         text += (char)key;
//         var width = Raylib.MeasureTextEx(font, text, fontSize, fontSpacing).X;
//         cursorPosition.X = width;
//         float zoomX = screenWidth / (width + 100);
//         float zoomY = screenHeight / (fontSize + 100);
//         camera.Zoom = Math.Min(zoomX, zoomY);
//         camera.Target = cursorPosition;
//         camera.Offset = new Vector2(screenWidth / 2, screenHeight / 2);
//         key = Raylib.GetCharPressed();
//     }
//     
//     if (Raylib.IsKeyPressed(KeyboardKey.Backspace))
//     {
//         text = text.Remove(text.Length - 1, 1);
//     }
//
//     
//     Raylib.BeginDrawing();
//     
//     Raylib.ClearBackground(Color.Black);
//
//     Raylib.BeginMode2D(camera);
//
//     Raylib.DrawTextEx(font, text, Vector2.Zero, fontSize, fontSpacing, Color.White);
//     
//     Raylib.EndMode2D();
//
//     Raylib.DrawText($"Camera: {camera.Target.X:0.0}, {camera.Target.Y:0.0}", screenWidth-200, screenHeight-40, 20, Color.White);
//     Raylib.DrawText($"Zoom: {camera.Zoom:0.00}x", screenWidth-200, screenHeight-20, 20, Color.White);
//     
//     Raylib.EndDrawing();
// }
//
// Raylib.CloseWindow();
