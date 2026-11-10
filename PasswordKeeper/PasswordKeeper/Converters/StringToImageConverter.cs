using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;

namespace PasswordKeeper.Converters;
public class StringToImageConverter : IValueConverter
{
    private static readonly ConcurrentDictionary<string, Bitmap> _bitmaps = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string path || string.IsNullOrWhiteSpace(path))
            return null;

        try
        {
            return _bitmaps.GetOrAdd(path, static p =>
            {
                using var stream = p.StartsWith("avares:", StringComparison.OrdinalIgnoreCase)
                    ? AssetLoader.Open(new Uri(p))
                    : File.OpenRead(p);
                return new Bitmap(stream);
            });
        }
        catch (Exception)
        {
            return null;
        }
    }

    // We don't need to convert back from image to path for this use case
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
