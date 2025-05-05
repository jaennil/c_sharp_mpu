using System;
using System.Formats.Tar;
using System.Globalization;
using Avalonia.Data.Converters;

namespace cards.Converters;

public class DoubleMultiplierConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        double inputValue = System.Convert.ToDouble(value);
        double multiplier = System.Convert.ToDouble(parameter);
        
        return inputValue * multiplier;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}