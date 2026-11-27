using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PasswordKeeper.Models.Password;
using PasswordKeeper.Navigation;
using PasswordKeeper.Services.Notifications;
using PasswordKeeper.Services.Passwords;

namespace PasswordKeeper.ViewModels;

public partial class PasswordViewModel(
    INavigator navigator,
    PasswordsContainer passwordsContainer,
    PasswordsRepository passwordsRepository,
    INotificationService notificationService
) : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayPassword))]
    private Password _password = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayPassword))]
    private bool _isPasswordVisible;

    public string DisplayPassword =>
        IsPasswordVisible ? Password.Value : new string('•', Password.Value.Length);

    [RelayCommand]
    private void OpenManager() =>
        navigator.NavigateTo<PasswordManagerViewModel, Password>(Password);

    [RelayCommand]
    private void TogglePasswordVisibility() => IsPasswordVisible = !IsPasswordVisible;

    [RelayCommand]
    private async Task Remove()
    {
        var result = await passwordsRepository.RemoveAsync(Password);
        if (result.IsSuccess)
            passwordsContainer.Remove(Password);

        notificationService.Show(result);
    }
}
