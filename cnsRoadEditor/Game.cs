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
			PromptRoadType();
		}
	}

	private Road PromptRoadType()
	{
		foreach(Road road in Enum.GetValues(typeof(Road)))
		{
			Console.WriteLine(road + ") " + road.GetView());
		}

		var road = PromptUser("Enter road type");
	}

	private int PromptUser(string entity)
	{
		Console.WriteLine("Enter {0}:");
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
