using static Raylib_cs.Raylib;
using static Raylib_cs.ConfigFlags;
using Raylib_cs;

namespace Game;

static class Game
{
    const int WindowHeight = 800;
    const int WindowWidth = 800;
    const int WindowMargin = 20;
    const string WindowTitle = "Soko";
    const int FontSize = 24;
    const int FontSizeLarge = 48;
    public static readonly Color BackgroundColor = Color.DarkGray;
    public static readonly Color TextColor = Color.RayWhite;

    const int UndoLimit = 50;

    private static Board? board;
    private static DropOutStack<Board> previousStates = new(UndoLimit);

    private static int currentLevel = 1;

    private enum Screen
    {
        Title,
        Gameplay,
        End
    }
    private static Screen currentScreen = Screen.Title;

    public static void Main()
    {
        InitWindow(WindowWidth, WindowHeight, WindowTitle);
        SetTargetFPS(144);

        ClearWindowState(ResizableWindow);

        // Load level 1.
        board = Levels.LoadLevel(currentLevel);
        SaveBoardState();

        while (!WindowShouldClose())
        {
            UpdateDrawFrame();
        }

        TextureManager.UnloadTextures();
        CloseWindow();
    }


    private static void UpdateDrawFrame()
    {
        Rectangle boardRect = GetCenteredBoardRect();

        BeginDrawing();

        ClearBackground(BackgroundColor);

        switch (currentScreen)
        {
            case Screen.Title:
                HandleKeyboardTitle();
                DrawText($"Soko", 5, 5, FontSizeLarge, TextColor);
                DrawText($"Press any key to start", 5, 5 + FontSizeLarge + 5, FontSize, TextColor);
                break;

            case Screen.Gameplay:
                HandleKeyboardGameplay();
                board?.Draw(boardRect);
                DrawText($"Level {currentLevel}", 5, 5, FontSize, TextColor);

                // Show info on controls during first level.
                if (currentLevel == 1)
                    DrawControls();
                break;

            case Screen.End:
                DrawText($"YOU WIN!", 5, 5, FontSizeLarge, TextColor);
                break;
        }



        EndDrawing();
    }

    private static void HandleKeyboardTitle()
    {
        var keyPress = (KeyboardKey)GetKeyPressed();
        if (keyPress != KeyboardKey.Null && keyPress != KeyboardKey.Escape)
        {
            currentScreen = Screen.Gameplay;
        }
    }

    private static void HandleKeyboardGameplay()
    {
        if (IsKeyPressed(KeyboardKey.Left) || IsKeyPressed(KeyboardKey.A))
            HandleMove(Direction.Left);
        if (IsKeyPressed(KeyboardKey.Right) || IsKeyPressed(KeyboardKey.D))
            HandleMove(Direction.Right);
        if (IsKeyPressed(KeyboardKey.Up) || IsKeyPressed(KeyboardKey.W))
            HandleMove(Direction.Up);
        if (IsKeyPressed(KeyboardKey.Down) || IsKeyPressed(KeyboardKey.S))
            HandleMove(Direction.Down);

        if (IsKeyPressed(KeyboardKey.R))
            LoadCurrentLevel(); // Reload level

        if (IsKeyPressed(KeyboardKey.Z))
            UndoAction();

    }

    private static void HandleMove(Direction dir)
    {
        SaveBoardState();
        board?.MovePlayers(dir);

        // Load next level if on win state.
        if (board?.IsInWinState() ?? false)
        {
            if (currentLevel == Levels.LastLevelIndex)
            {
                currentScreen = Screen.End;
                return;
            }

            currentLevel++;
            LoadCurrentLevel();
        }
    }

    private static void LoadCurrentLevel()
    {
        previousStates.Clear(); // Remove undo history.
        board = Levels.LoadLevel(currentLevel);
    }

    private static void SaveBoardState()
    {
        if (board is not null)
            previousStates.Push((Board)board.Clone());
    }

    private static void UndoAction()
    {
        if (!previousStates.CanPop()) return;

        board = previousStates.Pop();
    }

    private static void DrawControls()
    {
        DrawText("WASD/Arrow Keys to move, R to restart, ESC to exit",
                 5,
                 WindowHeight - 5 - FontSize,
                 FontSize,
                 Color.RayWhite);
    }

    /* Calculates a centered square to put the board in */
    private static Rectangle GetCenteredBoardRect()
    {
        int availableWidth = WindowWidth - 2 * WindowMargin;
        int availableHeight = WindowHeight - 2 * WindowMargin;
        int boardSize = Math.Min(availableWidth, availableHeight);

        int x = (WindowWidth - boardSize) / 2;
        int y = (WindowHeight - boardSize) / 2;

        return new Rectangle(x, y, boardSize, boardSize);
    }
}
