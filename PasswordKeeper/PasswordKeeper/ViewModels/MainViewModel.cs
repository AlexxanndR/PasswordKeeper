using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PasswordKeeper.Navigation;

namespace PasswordKeeper.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private INavigator _navigator;

    [ObservableProperty]
    private object? _currentPage;

    [ObservableProperty]
    private bool? _isDarkTheme = Application.Current?.ActualThemeVariant == ThemeVariant.Dark;

    public MainViewModel(INavigator navigator)
    {
        _navigator = navigator;
        _navigator.NavigationChanged += NavigationHandler;
        _navigator.NavigateTo<PasswordsListViewModel>();
    }

    private void NavigationHandler(object? sender, NavigationEventArgs args)
        => CurrentPage = args.NewViewModel;

    [RelayCommand]
    private void ToggleTheme()
    {
        IsDarkTheme = IsDarkTheme.HasValue && !IsDarkTheme.Value;
        if (Application.Current is { } app)
            Application.Current.RequestedThemeVariant = IsDarkTheme.HasValue && IsDarkTheme.Value ? ThemeVariant.Dark : ThemeVariant.Light;
    }
}
