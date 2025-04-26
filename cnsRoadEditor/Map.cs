class Map
{
	private char[,] _field;
	private int _width;
	private int _height;

	public Map(int width, int height)
	{
		_width = width;
		_height = height;
		_field = new char[_width, _height];
		FillField('o');
	}

	private void FillField(char c)
	{
		for (int i = 0; i < _width; i++)
		{
			for (int j = 0; j < _height; j++)
			{
				_field[i, j] = c;
			}
		}
	}

	public override string ToString()
	{
		string s = "";

		for (int i = 0; i < _width; i++)
		{
			for (int j = 0; j < _height; j++)
			{
				s += _field[i, j];
			}

			s += '\n';
		}

		return s;
	}

	public void SetRoad(Road road, int x, int y)
	{
		_field[x, y] = (char)road;
	}
}
