using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PasswordKeeper.Navigation;
using System.Collections.ObjectModel;

namespace PasswordKeeper.ViewModels;
public partial class PasswordsListViewModel(INavigator navigator) : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<PasswordViewModel> _passwords = [];

    [RelayCommand]
    private void AddPassword()
        => navigator.NavigateTo<PasswordManagerViewModel>();
}

