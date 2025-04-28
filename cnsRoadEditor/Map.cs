namespace cnsRoadEditor;

class Map
{
	private Road[,] _field;
	private int _width = 10;
	private int _height = 10;

	public Map(int? width, int? height)
	{
        if (width.HasValue)
        {
            _width = width.Value;
        }

        if (height.HasValue)
        {
            _height = height.Value;
        }

		_field = new Road[_height, _width];

		FillField();
	}

	private void FillField()
	{
		for (int i = 0; i < _height; i++)
		{
			for (int j = 0; j < _width; j++)
			{
				_field[i, j] = Road.None;
			}
		}
	}

	public override string ToString()
	{
		string s = "";

		for (int i = 0; i < _height; i++)
		{
			for (int j = 0; j < _width; j++)
			{
				s += _field[i, j].GetView();
			}

			s += '\n';
		}

		return s;
	}

	public void SetRoad(Road road, Point coordinate)
	{
		var x = coordinate.X;
		var y = coordinate.Y;
		_field[y, x] = road;

        CheckJunctions();
	}

	public void DrawRoadLine(Point startPoint, Point endPoint)
	{

        if (startPoint.X == endPoint.X)
        {
            DrawVerticalLine(startPoint.Y, endPoint.Y, startPoint.X);
            return;
        }
        if (startPoint.Y == endPoint.Y)
        {
            DrawHorizontalLine(startPoint.X, endPoint.X, startPoint.Y);
            return;
        }

        DrawHorizontalLine(startPoint.X, endPoint.X, startPoint.Y);
        DrawVerticalLine(startPoint.Y, endPoint.Y, endPoint.X);

        CheckJunctions();
	}

    private void DrawHorizontalLine(int startX, int endX, int y)
    {
        if (startX > endX)
        {
            (startX, endX) = (endX, startX);
        }

        for (int x = startX; x <= endX; x++)
        {
            var coordinate = new Point(x, y);
            SetRoad(Road.Horizontal, coordinate);
        }
    }

    private void DrawVerticalLine(int startY, int endY, int x)
    {
        if (startY > endY)
        {
            (startY, endY) = (endY, startY);
        }

        for (int y = startY; y <= endY; y++)
        {
            var coordinate = new Point(x, y);
            SetRoad(Road.Vertical, coordinate);
        }
    }

    private void CheckJunctions()
    {
        CheckRightTopCornerJunctions();
        CheckLeftTopCornerJunctions();
    }

    private void CheckRightTopCornerJunctions()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 1; x < _width - 1; x++)
            {
                if (_field[y, x] == Road.Horizontal)
                {
                    if (_field[y, x+1] == Road.Vertical)
                    {
                        _field[y, x+1] = Road.RightTopCorner;
                    }
                }
            }
        }

        for (int y = 0; y < _height - 1; y++)
        {
            for (int x = 1; x < _width; x++)
            {
                if (_field[y, x] == Road.Horizontal)
                {
                    if (_field[y+1, x] == Road.Vertical)
                    {
                        _field[y, x] = Road.RightTopCorner;
                    }
                }
            }
        }
    }

    private void CheckLeftTopCornerJunctions()
    {
        for (int y = 0; y < _height - 1; y++)
        {
            for (int x = 0; x < _width - 1; x++)
            {
                if (_field[y, x] == Road.Horizontal)
                {
                    if (_field[y+1, x] == Road.Vertical)
                    {
                        _field[y, x] = Road.LeftTopCorner;
                    }
                }
            }
        }

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width - 1; x++)
            {
                if (_field[y, x] == Road.Vertical)
                {
                    if (_field[y, x+1] == Road.Horizontal)
                    {
                        _field[y, x] = Road.LeftTopCorner;
                    }
                }
            }
        }
    }

    private void CheckBottomCornerJunctions()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width - 1; x++)
            {
                if (_field[y, x] == Road.Horizontal)
                {
                    if (_field[y, x+1] == Road.Vertical)
                    {
                        _field[y, x+1] = Road.RightTopCorner;
                    }
                }

                if (_field[y, x] == Road.Vertical)
                {
                    if (_field[y, x+1] == Road.Horizontal)
                    {
                        _field[y, x] = Road.LeftTopCorner;
                    }
                }
            }
        }
    }

	public void Clear()
	{
		FillField();
	}
}
