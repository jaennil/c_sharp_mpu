using System.Text.Unicode;

char[,]? GenMapFindTwins(int height = 9, int width = 9, string symbols = "10", int mode = 2)
{
    if (symbols.Length * mode > width * height)
    {
        Console.WriteLine("bad input");
        return null;
    }
    
    var map = new char[height, width];
    var rnd = new Random();
    var used = new List<(int, int)>();

    for (int i = 0; i < height; i++)
    {
        for (int j = 0; j < width; j++)
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
