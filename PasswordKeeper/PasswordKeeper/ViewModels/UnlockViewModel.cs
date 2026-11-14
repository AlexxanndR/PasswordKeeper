using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PasswordKeeper.Navigation;
using PasswordKeeper.Services.Database;
using PasswordKeeper.Services.Passwords;
using System.Threading.Tasks;

namespace PasswordKeeper.ViewModels;

public partial class UnlockViewModel(
    INavigator navigator,
    VaultManager vaultManager,
    PasswordsRepository passwordsRepository,
    PasswordsContainer passwordsContainer) : ViewModelBase
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UnlockCommand))]
    private string _masterPassword = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UnlockCommand))]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private bool _isPasswordRevealed;

    [ObservableProperty]
    private string? _error;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UnlockCommand))]
    private bool _isBusy;

    public bool IsFirstRun { get; } = vaultManager.IsFirstRun;

    private bool CanUnlock()
    {
        if (IsBusy)
            return false;

        if (string.IsNullOrEmpty(MasterPassword))
            return false;

        if (IsFirstRun && string.IsNullOrEmpty(ConfirmPassword))
            return false;

        return true;
    }

    [RelayCommand(CanExecute = nameof(CanUnlock))]
    private async Task Unlock()
    {
        Error = null;

        if (IsFirstRun && MasterPassword != ConfirmPassword)
        {
            Error = "Passwords don't match";
            return;
        }

        IsBusy = true;
        try
        {
            var result = await vaultManager.UnlockAsync(MasterPassword);

            if (!result.IsSuccess)
            {
                Error = result.Message;
                return;
            }

            passwordsContainer.Load(await passwordsRepository.LoadAllAsync());
            navigator.NavigateReplacing<PasswordsListViewModel>();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void TogglePasswordReveal()
        => IsPasswordRevealed = !IsPasswordRevealed;
}
