using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Material.Icons;

namespace PasswordKeeper.Converters;

public class ThemeToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isDarkTheme)
            return isDarkTheme ? MaterialIconKind.WeatherNight : MaterialIconKind.WeatherSunny;
        return MaterialIconKind.WeatherSunny;
    }

    // We don't need to convert back from icon to theme for this use case
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotSupportedException();
    }
}
