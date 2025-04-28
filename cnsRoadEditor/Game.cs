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
		Console.WriteLine(width);
		Console.WriteLine(height);
		_map = new Map(width, height);
	}

	private void MainLoop()
	{
		while (true)
		{
			var operation = PromptOperation();
			switch (operation)
			{
				case Operation.Point:
					var road = PromptRoadType();
					int x = PromptUser("x coordinate");
					int y = PromptUser("y coordinate");
					_map.SetRoad(road, x, y);
					break;
				case Operation.PrintMap:
					Console.WriteLine(_map);
					break;
				case Operation.SaveToFile:
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
			PromptUser(entity);
		}

		return intInput;
	}
}
