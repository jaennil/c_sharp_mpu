namespace cnsRoadEditor;

class Game
{
	private Map _map;

	public void Run()
	{
		InitMap();
		MainLoop();
	}

	private void InitMap()
	{
		var width = PromptUser("map width");
		var height = PromptUser("map height");
		_map = new Map(width, height);
	}

	private void MainLoop()
	{
		while (true)
		{
			var operation = PromptOperation();
			
			switch (operation)
			{
				case Operation.PrintMap:
					Console.WriteLine(_map);
					break;
				case Operation.Point:
					var road = PromptRoadType();
					int x = PromptUser("x coordinate");
					int y = PromptUser("y coordinate");
					var coordinate = new Point(x, y);
					_map.SetRoad(road, coordinate);
					break;
				case Operation.Line:
					var startX = PromptUser("start x coordinate");
					var startY = PromptUser("start y coordinate");
					var endX = PromptUser("end x coordinate");
					var endY = PromptUser("end y coordinate");
					var startPoint = new Point(startX, startY);
					var endPoint = new Point(endX, endY);
					_map.DrawRoadLine(startPoint, endPoint);
					break;
				case Operation.Clear:
					_map.Clear();
					break;
				case Operation.SaveToFile:
					using (var sw = new StreamWriter("output.txt"))
					{
						sw.Write(_map.ToString());
						sw.Flush();
						sw.Close();
					}
					break;
			}
		}
	}

	private Operation PromptOperation()
	{
		Console.WriteLine("Available operations:");
		foreach (Operation op in Enum.GetValues(typeof(Operation)))
		{
			Console.WriteLine((int)op + ") " + op);
		}

		var operation = PromptUser("operation");
		return (Operation)operation;
	}

	private Road PromptRoadType()
	{
		Console.WriteLine("Available roads:");
		foreach(Road road in Enum.GetValues(typeof(Road)))
		{
			Console.WriteLine("{0}) {1} ({2})", (int)road, road, road.GetView());
		}

		var roadUserInput = PromptUser("road type");
		return (Road)roadUserInput;
	}

	private int PromptUser(string entity)
	{
		Console.WriteLine("Enter {0}:", entity);
		Console.Write("> ");

		string input = Console.ReadLine();

		int intInput;
		if (Int32.TryParse(input, out intInput) == false)
		{
			Console.WriteLine("ERROR: {0} must be positive integer", entity);
			return PromptUser(entity);
		}

		return intInput;
	}
}
