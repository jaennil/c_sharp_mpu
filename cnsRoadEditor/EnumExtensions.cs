public static class EnumExtensions
{
	public static string GetView(this Enum value)
	{
		var field = value.GetType().GetField(value.ToString());
		var attribute = field?.GetCustomAttribute<ViewAttribute>();
		return attribute?.View ?? value.ToString();
	}
}
