using System.Text.Unicode;

char[,]? GenMapFindTwins(int width = 9, int height = 9, string symbols = "10", int mode = 2)
{
    if (symbols.Length * mode > width * height)
    {
        Console.WriteLine("bad input");
        return null;
    }
    
    var map = new char[width, height];
    var rnd = new Random();
    var used = new List<(int, int)>();

    for (int i = 0; i < width; i++)
    {
        for (int j = 0; j < height; j++)
        {
            map[i, j] = '-';
        }
    }

    for (int i = 0; i < symbols.Length; i++)
    {
        for (int j = 0; j < mode; j++)
        {
            var x = rnd.Next(0, width);
            var y = rnd.Next(0, height);

            while (used.Contains((x, y)))
            {
                x = rnd.Next(0, width);
                y = rnd.Next(0, height);
            }

            map[x, y] = symbols[i];
            used.Add((x, y));
        }
    }
    
    return map;
}

void Print2DArray(char[,] a)
{
    for (var i = 0; i < a.GetLength(0); i++)
    {
        for (var j = 0; j < a.GetLength(1); j++)
        {
            Console.Write(a[i, j]);
        }
        Console.WriteLine();
    }
}

var map = GenMapFindTwins();
if (map != null)
{
    Print2DArray(map);
}
Console.WriteLine();

var map2 = GenMapFindTwins(height: 5, width: 10, symbols: "ABC", mode: 3);
if (map2 != null)
{
    Print2DArray(map2);
}
Console.WriteLine();

var map3 = GenMapFindTwins(height: 2, width: 2, symbols: "X", mode: 1);
if (map3 != null)
{
    Print2DArray(map3);
}
Console.WriteLine();

// Should error
var map4 = GenMapFindTwins(height: 3, width: 3, symbols: "123", mode: 4);
Console.WriteLine();

var map5 = GenMapFindTwins(height: 4, width: 4, symbols: "♥♦", mode: 4);
if (map5 != null)
{
    Print2DArray(map5);
}
Console.WriteLine();

var map6 = GenMapFindTwins(height: 3, width: 3, symbols: "A", mode: 9);
if (map6 != null)
{
    Print2DArray(map6);
}
