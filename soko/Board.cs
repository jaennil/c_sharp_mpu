using Raylib_cs;

namespace Game;

public class Board : ICloneable
{
    private GridCell[,] grid;

    public Board(GridCell[,] grid)
    {
        this.grid = grid;
    }

    public void MovePlayers(Direction dir)
    {
        foreach (var playerPos in FindObjects(SurfaceObject.Player)) // Would not work properly for multiple players
            MovePlayer(playerPos, dir);
    }

    public bool IsInWinState()
    {
        var buttons = FindObjects(FloorObject.Button).Select(pos => GetCell(pos));
        // Check there are boxes on every button.
        if (!buttons.All(cell => cell.HasObject(SurfaceObject.Box))) return false;

        var goals = FindObjects(FloorObject.Goal).Select(pos => GetCell(pos));
        // Check there is a player on a flag.
        if (!goals.Any(cell => cell.HasObject(SurfaceObject.Player))) return false;

        return true;
    }

    public void Draw(Rectangle rect)
    {
        // Calculate tile size
        int tileSize = (int)Math.Min(rect.Width / grid.GetLength(0), rect.Height / grid.GetLength(1));

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                Rectangle tileRect = new Rectangle(rect.X + x * tileSize, rect.Y + y * tileSize, tileSize, tileSize);
                grid[x, y].Draw(tileRect);
            }
        }
    }

    public object Clone()
    {
        var newGrid = new GridCell[grid.GetLength(0), grid.GetLength(1)];
        for (int x = 0; x < grid.GetLength(0); x++) // Deep copy the 2D array
            for (int y = 0; y < grid.GetLength(1); y++)
                newGrid[x, y] = (GridCell)grid[x, y].Clone();

        return new Board(newGrid);
    }

    private List<Position> FindObjects(SurfaceObject objectType)
    {
        var list = new List<Position>();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y].HasObject(objectType))
                {
                    list.Add(new Position { X = x, Y = y });
                }
            }
        }

        return list;
    }

    private List<Position> FindObjects(FloorObject objectType)
    {
        var list = new List<Position>();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y].HasObject(objectType))
                {
                    list.Add(new Position { X = x, Y = y });
                }
            }
        }

        return list;
    }

    private void MovePlayer(Position playerPos, Direction dir)
    {
        Position nextPos = playerPos.AddDisplacement(dir);
        GridCell targetCell = GetCell(nextPos);

        if (!IsInsideBoard(nextPos) || targetCell.HasObject(SurfaceObject.Wall)) return; // Don't move

        if (targetCell.HasObject(SurfaceObject.Box))
        {
            if (!CanPush(nextPos, dir)) return; // Don't move

            MoveSurfaceObject(nextPos, dir); // Move box and continue to move the player.
        }

        // Move the player.
        MoveSurfaceObject(playerPos, dir);
    }

    private bool CanPush(Position pos, Direction dir)
    {
        Position nextPos = pos.AddDisplacement(dir);
        GridCell targetCell = GetCell(nextPos);

        // No multipush :)
        if (targetCell.HasObject(SurfaceObject.Wall) || targetCell.HasObject(SurfaceObject.Box)) return false;

        return true;

    }

    private void MoveSurfaceObject(Position pos, Direction dir)
    {
        Position nextPos = pos.AddDisplacement(dir);
        GridCell sourceCell = GetCell(pos);
        GridCell targetCell = GetCell(nextPos);

        targetCell.surfaceObject = sourceCell.surfaceObject;
        sourceCell.surfaceObject = null;
    }

    private bool IsInsideBoard(Position pos)
    {
        return pos.X >= 0 && pos.X < grid.GetLength(0) && pos.Y >= 0 && pos.Y < grid.GetLength(1);
    }

    private GridCell GetCell(Position pos) => grid[pos.X, pos.Y];
}

public class GridCell : ICloneable
{
    public FloorObject? floorObject { get; }
    public SurfaceObject? surfaceObject { get; set; }

    public GridCell(FloorObject? floorObject, SurfaceObject? surfaceObject)
    {
        this.floorObject = floorObject;
        this.surfaceObject = surfaceObject;
    }

    public void Draw(Rectangle rectangle)
    {
        // Draw floor object
        if (floorObject is not null)
            TextureManager.DrawInRectangle(floorObject.Value, rectangle);

        // Draw surface object above it
        if (surfaceObject is not null)
            TextureManager.DrawInRectangle(surfaceObject.Value, rectangle);
    }

    public bool HasObject(SurfaceObject obj) => surfaceObject == obj;
    public bool HasObject(FloorObject obj) => floorObject == obj;

    public object Clone()
    {
        return new GridCell(this.floorObject, this.surfaceObject);
    }
}

public enum FloorObject
{
    Floor,
    Button,
    Goal,
}

public enum SurfaceObject
{
    Wall,
    Box,
    Player
}
