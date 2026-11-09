using Avalonia.Data.Converters;
using Material.Icons;
using System;
using System.Globalization;

namespace PasswordKeeper.Converters;
public class PasswordRevealToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isRevealed)
            return isRevealed ? MaterialIconKind.EyeOff : MaterialIconKind.Eye;
        return MaterialIconKind.Eye;
    }

    // We don't need to convert back from icon to reveal state for this use case
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
