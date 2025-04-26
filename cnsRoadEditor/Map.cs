namespace cnsRoadEditor;

class Map
{
	private string[,] _field;
	private int _width;
	private int _height;

	public Map(int width, int height)
	{
		_width = width;
		_height = height;
		_field = new string[height, width];
		FillField("o");
	}

	private void FillField(string c)
	{
		for (int i = 0; i < _height; i++)
		{
			for (int j = 0; j < _width; j++)
			{
				_field[i, j] = c;
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

	public void SetRoad(Road road, int x, int y)
	{
		_field[y, x] = road.GetView();
	}
}
