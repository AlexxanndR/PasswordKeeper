using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PasswordKeeper.Models.Password;
using PasswordKeeper.Navigation;

namespace PasswordKeeper.ViewModels;
public partial class PasswordViewModel(INavigator navigator) : ViewModelBase
{
    [ObservableProperty]
    private Password _password = new();

    [RelayCommand]
    private void OpenManager() 
        => navigator.NavigateTo<PasswordManagerViewModel>();
}
