using System;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Platform;

namespace PasswordKeeper.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ApplyThemeIcon();
        ActualThemeVariantChanged += (_, _) => ApplyThemeIcon();
    }

    private void ApplyThemeIcon()
    {
        var uri = ActualThemeVariant == ThemeVariant.Dark
            ? "avares://PasswordKeeper/Assets/Logo/icon-light.ico"
            : "avares://PasswordKeeper/Assets/Logo/icon.ico";
        Icon = new WindowIcon(AssetLoader.Open(new Uri(uri)));
    }
}