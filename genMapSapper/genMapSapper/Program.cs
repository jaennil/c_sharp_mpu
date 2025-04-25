string[,]? GenMapSapper(int height = 9, int width = 9, int randomMinesAmount = 10,  (int, int)[]? minesCoordinates = null, (int, int)? firstMoveCoordinates = null)
{
    var map = new string[width, height];
    var rnd = new Random();
    var mines = new List<(int, int)>();
    var cellsAmount = height * width;
    var minesCoordinatesAmount = 0;
    
    if (minesCoordinates != null)
    {
        minesCoordinatesAmount = minesCoordinates.GetLength(0);
    }
    
    var minesAmount = minesCoordinatesAmount + randomMinesAmount;
    
    if (cellsAmount < minesAmount)
    {
        Console.WriteLine("mines amount should be less than total cells, got: %v random mines; %v mines by coordinates; %v total cells");
        return null;
    }

    for (var i = 0; i < minesCoordinatesAmount; i++)
    {
        var x = minesCoordinates[i].Item1;
        var y = minesCoordinates[i].Item2;
        if ((x, y) == firstMoveCoordinates)
        {
            Console.WriteLine("WARNING(GenMapSwagger): bad input");
            continue;
        }
        map[x, y] = "*";
        mines.Add((x, y));
    }


    for (var i = 0; i < randomMinesAmount; i++)
    {
        var x = rnd.Next(0, width);
        var y = rnd.Next(0, height);
        
        while (mines.Contains((x, y)) || (x, y) == firstMoveCoordinates)
        {
            x = rnd.Next(0, width);
            y = rnd.Next(0, height);
        }
        
        map[x, y] = "*";
        mines.Add((x, y));
    }

    for (var i = 0; i < width; i++)
    {
        for (var j = 0; j < height; j++)
        {
            if (map[i, j] == "*")
            {
                continue;
            }
            
            var minesAroundAmount = 0;
            for (var k = i - 1; k <= i + 1; k++)
            {
                for (var l = j - 1; l <= j + 1; l++)
                {
                    if (k < 0 || k >= width || l < 0 || l >= height)
                    {
                        continue;
                    }
                    
                    if (map[k, l] == "*")
                    {
                        minesAroundAmount++;
                    }
                }
            }
            
            map[i, j] = minesAroundAmount.ToString();
        }
    }
    
    return map;
}

void Print2DArray(string[,] a)
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

var map1 = GenMapSapper();
if (map1 != null)
{
    Print2DArray(map1);
}
Console.WriteLine();

var map2 = GenMapSapper(height: 5, width: 5, randomMinesAmount: 3);
if (map2 != null)
{
    Print2DArray(map2);
}
Console.WriteLine();

var map3 = GenMapSapper(
    minesCoordinates: new[] {(1, 1), (2, 2), (3, 3)},
    firstMoveCoordinates: (0, 0)
);
if (map3 != null)
{
    Print2DArray(map3);
}
Console.WriteLine();

var map4 = GenMapSapper(
    height: 8,
    width: 8,
    randomMinesAmount: 5,
    minesCoordinates: new[] {(0, 0), (7, 7)},
    firstMoveCoordinates: (4, 4)
);
if (map4 != null)
{
    Print2DArray(map4);
}
Console.WriteLine();

var map5 = GenMapSapper(
    height: 20,
    width: 20,
    randomMinesAmount: 50
);
if (map5 != null)
{
    Print2DArray(map5);
}
Console.WriteLine();

var map6 = GenMapSapper(
    height: 3,
    width: 3,
    randomMinesAmount: 10
);
if (map6 != null)
{
    Print2DArray(map6);
}
Console.WriteLine();

var map7 = GenMapSapper(
    height: 10,
    width: 10,
    randomMinesAmount: 11,
    minesCoordinates: new[] {(3, 3), (4, 4)},
    firstMoveCoordinates: (1, 1)
);
if (map7 != null)
{
    Print2DArray(map7);
}
