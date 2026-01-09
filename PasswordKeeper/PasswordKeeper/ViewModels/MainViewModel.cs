using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PasswordKeeper.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool? _isDarkTheme = Application.Current?.ActualThemeVariant == ThemeVariant.Dark;

    [RelayCommand]
    private void ToggleTheme()
    {
        if (Application.Current is { } app)
            Application.Current.RequestedThemeVariant = IsDarkTheme.HasValue && IsDarkTheme.Value ? ThemeVariant.Dark : ThemeVariant.Light;
    }
}
