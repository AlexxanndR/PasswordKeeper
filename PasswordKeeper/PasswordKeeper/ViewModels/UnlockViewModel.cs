using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using PasswordKeeper.Navigation;
using PasswordKeeper.Services.Database;
using PasswordKeeper.Services.Passwords;
using System.Threading.Tasks;

namespace PasswordKeeper.ViewModels;

public partial class UnlockViewModel(
    INavigator navigator,
    VaultManager vaultManager,
    PasswordsRepository passwordsRepository,
    PasswordsContainer passwordsContainer,
    IValidator<UnlockViewModel> validator) : ValidatableViewModelBase<UnlockViewModel>(validator)
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
    [NotifyCanExecuteChangedFor(nameof(UnlockCommand))]
    private bool _isBusy;

    public bool IsFirstRun { get; } = vaultManager.IsFirstRun;

    private bool CanUnlock()
        => !IsBusy && IsValid;

    [RelayCommand(CanExecute = nameof(CanUnlock))]
    private async Task Unlock()
    {
        IsBusy = true;
        try
        {
            // Runs on the thread pool: UnlockAsync does heavy CPU work (PBKDF2)
            // before its first real await, which would block the UI thread and
            // prevent the busy spinner from ever rendering
            var result = await Task.Run(() => vaultManager.UnlockAsync(MasterPassword));

            if (!result.IsSuccess)
            {
                SetError(nameof(MasterPassword), result.Message);
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
