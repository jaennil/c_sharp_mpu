using System.Reflection;
using System.ComponentModel;

namespace cnsRoadEditor;

public static class EnumExtensions
{
	public static string GetView(this Enum value)
	{
		var field = value.GetType().GetField(value.ToString());
		var attribute = field?.GetCustomAttribute<ViewAttribute>();
		return attribute?.View ?? value.ToString();
	}
}

[AttributeUsage(AttributeTargets.Field)]
public class ViewAttribute : Attribute
{
	public string View { get; }

	public ViewAttribute(string view)
	{
		View = view;
	}
}
