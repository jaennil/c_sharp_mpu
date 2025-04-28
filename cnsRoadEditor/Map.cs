namespace cnsRoadEditor;

class Map
{
	private string[,] _field;
	private int _width;
	private int _height;
	private const string EmptyChar = "\u22c5";

	public Map(int width, int height)
	{
		_width = width;
		_height = height;
		_field = new string[height, width];
		FillField();
	}

	private void FillField()
	{
		for (int i = 0; i < _height; i++)
		{
			for (int j = 0; j < _width; j++)
			{
				_field[i, j] = EmptyChar;
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
				s += _field[i, j];
			}

			s += '\n';
		}

		return s;
	}

	public void SetRoad(Road road, Point coordinate)
	{
		var x = coordinate.X;
		var y = coordinate.Y;
		_field[y, x] = road.GetView();
	}

	public void DrawRoadLine(Point startPoint, Point endPoint)
	{
		var minX = Math.Min(startPoint.X, endPoint.X);
		var maxX = Math.Max(startPoint.X, endPoint.X);
		var minY = Math.Min(startPoint.Y, endPoint.Y);
		var maxY = Math.Max(startPoint.Y, endPoint.Y);
		
		for (int x = minX; x <= maxX; x++)
		{
			_field[minY, x] = Road.Horizontal.GetView();
		}
		
		for (int y = minY; y <= maxY; y++)
		{
			_field[y, maxX] = Road.Vertical.GetView();
		}
	}

	public void Clear()
	{
		FillField();
	}
}
