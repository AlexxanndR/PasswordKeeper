using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PasswordKeeper.Navigation;
using PasswordKeeper.Services.Passwords;

namespace PasswordKeeper.ViewModels;

public partial class PasswordsListViewModel : ViewModelBase
{
    private readonly INavigator _navigator;
    private readonly PasswordsContainer _passwordsContainer;

    [ObservableProperty]
    private ObservableCollection<PasswordViewModel> _passwords;

    public PasswordsListViewModel(INavigator navigator, PasswordsContainer passwordsContainer)
    {
        _navigator = navigator;
        _passwordsContainer = passwordsContainer;
        _passwords = passwordsContainer.Passwords;

        _navigator.NavigationChanged += OnNavigationChanged;
    }

    private void OnNavigationChanged(object? sender, NavigationEventArgs args)
    {
        if (ReferenceEquals(args.NewViewModel, this))
            Passwords = _passwordsContainer.Passwords;
    }

    [RelayCommand]
    private void AddPassword() => _navigator.NavigateTo<PasswordManagerViewModel>();
}
