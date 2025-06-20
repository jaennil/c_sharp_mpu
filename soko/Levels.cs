namespace Game;

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

// Helpers for loading levels from disk
public static class Levels
{
    public static Board LoadLevel(int levelId)
    {
        return FromCsv(levelPaths[levelId]);
    }

    private static readonly Dictionary<int, string> levelPaths = new()
    {
        [1] = "resources/levels/level1",
        [2] = "resources/levels/level2",
        [3] = "resources/levels/level3"
    };
    public const int LastLevelIndex = 3;

    private static Board FromCsv(string basePath)
    {
        // basePath example: "data/levels/level1"
        var pathFloor = $"{basePath}_floor.csv";
        var pathSurface = $"{basePath}_surface.csv";

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            NewLine = Environment.NewLine,
            HasHeaderRecord = false
        };
        // Open and create readers for both csv files
        using (var readerFloor = new StreamReader(pathFloor))
        using (var csvFloor = new CsvReader(readerFloor, config))
        using (var readerSurface = new StreamReader(pathSurface))
        using (var csvSurface = new CsvReader(readerSurface, config))
        {
            var recordsFloor = csvFloor.GetRecords<CsvRow>().ToList();
            var recordsSurface = csvSurface.GetRecords<CsvRow>().ToList();

            // Check both map layers have the same dimensions
            if (recordsFloor.Count != recordsSurface.Count ||
                recordsFloor[0].Values.Count != recordsSurface[0].Values.Count)
                throw new InvalidLevelFilesException("The CSV files for the level do not have the same dimensions");

            var boardGrid = new GridCell[recordsFloor[0].Values.Count, recordsFloor.Count];
            for (int y = 0; y < recordsFloor.Count; y++)
            {
                var floorRow = recordsFloor[y].Values;
                var surfaceRow = recordsSurface[y].Values;
                for (int x = 0; x < floorRow.Count; x++)
                {
                    boardGrid[x, y] = new GridCell(idToFloorElement[floorRow[x]],
                                                   idToSurfaceElement[surfaceRow[x]]);
                }
            }
            return new Board(boardGrid);
        }
    }

    private static readonly Dictionary<int, FloorObject?> idToFloorElement = new()
    {
        [-1] = null,
        [0] = FloorObject.Floor,
        [4] = FloorObject.Button,
        [5] = FloorObject.Goal
    };
    private static readonly Dictionary<int, SurfaceObject?> idToSurfaceElement = new()
    {
        [-1] = null,
        [1] = SurfaceObject.Wall,
        [2] = SurfaceObject.Player,
        [3] = SurfaceObject.Box
    };

    private class CsvRow
    {
        public List<int> Values { get; set; } = new List<int>();
    }
}

public class InvalidLevelFilesException : Exception
{
    public InvalidLevelFilesException(string msg) : base(msg) { }
}
