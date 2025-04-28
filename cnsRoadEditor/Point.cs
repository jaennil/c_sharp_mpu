namespace cnsRoadEditor;

public class Point
{
    public int X = 0;
    public int Y = 0;

    public Point(int? x, int? y)
    {
        if (x.HasValue)
        {
            X = x.Value;
        }

        if (y.HasValue)
        {
            Y = y.Value;
        }
    }
}
