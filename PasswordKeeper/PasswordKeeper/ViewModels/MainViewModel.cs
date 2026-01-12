using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PasswordKeeper.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool? _isDarkTheme = Application.Current?.ActualThemeVariant == ThemeVariant.Dark;

    [ObservableProperty]
    private UserControl? _currentView;

    [RelayCommand]
    private void ToggleTheme()
    {
        IsDarkTheme = IsDarkTheme.HasValue && !IsDarkTheme.Value;
        if (Application.Current is { } app)
            Application.Current.RequestedThemeVariant = IsDarkTheme.HasValue && IsDarkTheme.Value ? ThemeVariant.Dark : ThemeVariant.Light;
    }

    [RelayCommand]
    private void AddPassword()
    {
        CurrentView = new PasswordManagerView();
    }
}
