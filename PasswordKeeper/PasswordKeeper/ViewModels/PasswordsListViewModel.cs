using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PasswordKeeper.Navigation;
using PasswordKeeper.Services;
using System.Collections.ObjectModel;

namespace PasswordKeeper.ViewModels;
public partial class PasswordsListViewModel(INavigator navigator, PasswordsContainer passwordsContainer) : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<PasswordViewModel> _passwords = passwordsContainer.Passwords;

    [RelayCommand]
    private void AddPassword()
        => navigator.NavigateTo<PasswordManagerViewModel>();
}

