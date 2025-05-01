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

    // Проверяем, что координаты находятся в пределах поля
    // if (x < 0 || x >= _field.GetLength(1) || y < 0 || y >= _field.GetLength(0))
    //     return;

    // Проверяем соседние клетки (сверху, снизу, слева, справа)
    bool hasTop = y > 0 && (_field[y - 1, x] == Road.Vertical || _field[y - 1, x] == Road.Cross || 
                            _field[y - 1, x] == Road.TopT);
    bool hasBottom = y < _field.GetLength(0) - 1 && (_field[y + 1, x] == Road.Vertical || _field[y + 1, x] == Road.Cross || 
                             _field[y + 1, x] == Road.BottomT);
    bool hasLeft = x > 0 && (_field[y, x - 1] == Road.Horizontal || _field[y, x - 1] == Road.Cross || 
                             _field[y, x - 1] == Road.RightT);
    bool hasRight = x < _field.GetLength(1) - 1 && (_field[y, x + 1] == Road.Horizontal || _field[y, x + 1] == Road.Cross || 
                             _field[y, x + 1] == Road.LeftT);

    if (road == Road.Horizontal)
    {
        // Если есть соединения сверху или снизу, это может быть перекресток или Т-образное соединение
        if (hasTop || hasBottom)
        {
            if (hasTop && hasBottom)
            {
                road = Road.Cross; // Перекресток (┼)
            }
            else if (hasTop)
            {
                road = Road.BottomT; // Т-образное соединение (┬)
            }
            else if (hasBottom)
            {
                road = Road.TopT; // Т-образное соединение (┴)
            }
        }
        // Если есть соединения слева и справа, это горизонтальная линия
        else if (hasLeft || hasRight)
        {
            road = Road.Horizontal; // Просто горизонтальная дорога (─)
        }
    }
    else if (road == Road.Vertical)
    {
        // Если есть соединения слева или справа, это может быть перекресток или Т-образное соединение
        if (hasLeft || hasRight)
        {
            if (hasLeft && hasRight)
            {
                road = Road.Cross; // Перекресток (┼)
            }
            else if (hasLeft)
            {
                road = Road.RightT; // Т-образное соединение (┤)
            }
            else if (hasRight)
            {
                road = Road.LeftT; // Т-образное соединение (├)
            }
        }
        // Если есть соединения сверху и снизу, это вертикальная линия
        else if (hasTop || hasBottom)
        {
            road = Road.Vertical; // Просто вертикальная дорога (│)
        }
    }

    // Устанавливаем дорогу в поле
    _field[y, x] = road;
}

        // if (road == Road.Horizontal)
        // {
        //     if (x + 2 < _width)
        //     {
        //         if (_field[y, x+1] == Road.Vertical)
        //         {
        //             if(_field[y, x+2] == Road.Horizontal)
        //             {
        //                 _field[y, x+1] = Road.Cross;
        //             }
        //         }
        //     }
        // }
        //
        // if (road == Road.Vertical)
        // {
        //     if (y + 2 < _height)
        //     {
        //         if (_field[y+1, x] == Road.Horizontal)
        //         {
        //             if(_field[y+2, x] == Road.Vertical)
        //             {
        //                 _field[y+1, x] = Road.Cross;
        //             }
        //         }
        //     }
        // }

        // CheckJunctions();
	// }

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

    // private void CheckJunctions()
    // {
        // CheckRightTopCornerJunctions();
        // CheckLeftTopCornerJunctions();
        // CheckRightBottomCornerJunctions();
    //     CheckCrossJunctions();
    // }

    // private void CheckCrossJunctions()
    // {
    //     for (int y = 0; y < _height - 2; y++)
    //     {
    //         for (int x = 0; x < _width; x++)
    //         {
    //             if (_field[y, x] == Road.Vertical)
    //             {
    //                 if (_field[y+1, x] == Road.Horizontal)
    //                 {
    //                     if (_field[y+2, x] == Road.Vertical)
    //                     {
    //                         _field[y+1, x] = Road.Cross;
    //                     }
    //                 }
    //             }
    //         }
    //     }
    //
    //     for (int y = 0; y < _height; y++)
    //     {
    //         for (int x = 0; x < _width - 2; x++)
    //         {
    //             if (_field[y, x] == Road.Horizontal)
    //             {
    //                 if (_field[y, x+1] == Road.Vertical)
    //                 {
    //                     if (_field[y, x+2] == Road.Horizontal)
    //                     {
    //                         _field[y, x+1] = Road.Cross;
    //                     }
    //                 }
    //             }
    //         }
    //     }
    // }

    // private void CheckRightTopCornerJunctions()
    // {
    //     for (int y = 0; y < _height - 1; y++)
    //     {
    //         for (int x = 1; x < _width - 1; x++)
    //         {
    //             if (_field[y, x] == Road.Horizontal)
    //             {
    //                 if (_field[y, x+1] == Road.Vertical)
    //                 {
    //                     _field[y, x+1] = Road.RightTopCorner;
    //                 }
    //             }
    //         }
    //
    //         for (int x = 1; x < _width; x++)
    //         {
    //             if (_field[y, x] == Road.Horizontal)
    //             {
    //                 if (_field[y+1, x] == Road.Vertical)
    //                 {
    //                     _field[y, x] = Road.RightTopCorner;
    //                 }
    //             }
    //         }
    //     }
    // }
    //
    // private void CheckLeftTopCornerJunctions()
    // {
    //     for (int y = 0; y < _height - 1; y++)
    //     {
    //         for (int x = 0; x < _width - 1; x++)
    //         {
    //             if (_field[y, x] == Road.Vertical)
    //             {
    //                 if (_field[y, x+1] == Road.Horizontal)
    //                 {
    //                     _field[y, x] = Road.LeftTopCorner;
    //                 }
    //             }
    //
    //             if (_field[y, x] == Road.Horizontal)
    //             {
    //                 if (_field[y+1, x] == Road.Vertical)
    //                 {
    //                     _field[y, x] = Road.LeftTopCorner;
    //                 }
    //             }
    //         }
    //     }
    // }
    //
    // private void CheckRightBottomCornerJunctions()
    // {
    //     for (int y = 1; y < _height - 1; y++)
    //     {
    //         for (int x = 1; x < _width; x++)
    //         {
    //             if (_field[y, x] == Road.Vertical)
    //             {
    //                 if (_field[y+1, x] == Road.Horizontal)
    //                 {
    //                     _field[y+1, x] = Road.RightBottomCorner;
    //                 }
    //             }
    //         }
    //     }
    //
    //     for (int y = 1; y < _height; y++)
    //     {
    //         for (int x = 1; x < _width - 1; x++)
    //         {
    //             if (_field[y, x] == Road.Horizontal)
    //             {
    //                 if (_field[y, x+1] == Road.Vertical)
    //                 {
    //                     _field[y, x+1] = Road.RightBottomCorner;
    //                 }
    //             }
    //         }
    //     }
    // }
    //
    // private void CheckLeftBottomCornerJunctions()
    // {
    //     for (int y = 1; y < _height; y++)
    //     {
    //         for (int x = 0; x < _width - 1; x++)
    //         {
    //             // if (_field[
    //         }
    //     }
    // }

	public void Clear()
	{
		FillField();
	}
}
