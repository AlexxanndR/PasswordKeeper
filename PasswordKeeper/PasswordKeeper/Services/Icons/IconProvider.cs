using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PasswordKeeper.Services.Icons;
public static class IconProvider
{
    private const string IconsFolder = "avares://PasswordKeeper/Assets/Icons";
    private const string DefaultIconFileName = "globe.png";

    private static readonly IReadOnlyList<string> _icons =
        [.. AssetLoader.GetAssets(new Uri(IconsFolder), null)
            .Select(uri => uri.ToString())
            .OrderBy(icon => IsDefaultIcon(icon) ? 0 : 1)
            .ThenBy(icon => icon, StringComparer.OrdinalIgnoreCase)];

    public static IReadOnlyList<string> Icons => _icons;

    public static string? DefaultIcon { get; } = _icons.FirstOrDefault(IsDefaultIcon);

    private static bool IsDefaultIcon(string icon)
        => icon.EndsWith("/" + DefaultIconFileName, StringComparison.OrdinalIgnoreCase);
}
