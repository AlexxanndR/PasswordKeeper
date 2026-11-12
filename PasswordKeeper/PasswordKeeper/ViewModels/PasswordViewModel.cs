using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PasswordKeeper.Models.Password;
using PasswordKeeper.Navigation;

namespace PasswordKeeper.ViewModels;
public partial class PasswordViewModel(INavigator navigator) : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayPassword))]
    private Password _password = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayPassword))]
    private bool _isPasswordVisible;

    public string DisplayPassword
        => IsPasswordVisible ? Password.Value : new string('•', Password.Value.Length);

    [RelayCommand]
    private void OpenManager() 
        => navigator.NavigateTo<PasswordManagerViewModel>();

    [RelayCommand]
    private void TogglePasswordVisibility()
        => IsPasswordVisible = !IsPasswordVisible;
}
