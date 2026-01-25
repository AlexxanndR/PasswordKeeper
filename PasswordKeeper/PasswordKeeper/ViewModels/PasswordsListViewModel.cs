using CommunityToolkit.Mvvm.Input;
using PasswordKeeper.Navigation;

namespace PasswordKeeper.ViewModels;
public partial class PasswordsListViewModel(INavigator navigator) : ViewModelBase
{
    [RelayCommand]
    private void AddPassword()
        => navigator.NavigateTo<PasswordManagerViewModel>();
}

