using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PasswordKeeper.Models.Password;
using PasswordKeeper.Navigation;
using PasswordKeeper.Services.Icons;
using PasswordKeeper.Services.Passwords;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PasswordKeeper.ViewModels;
public partial class PasswordManagerViewModel(
    INavigator navigator,
    IconStore iconStore,
    IconImporter iconImporter,
    PasswordsContainer passwordsContainer) : ViewModelBase
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _name = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _passwordValue = string.Empty;

    [ObservableProperty]
    private bool _isPasswordRevealed;

    [ObservableProperty]
    private bool _isIconPickerOpen;

    [ObservableProperty]
    private string? _selectedIcon = IconProvider.DefaultIcon;

    [ObservableProperty]
    private string? _importError;

    [ObservableProperty]
    private string? _importSuccess;

    public ObservableCollection<string> AvailableIcons { get; } = [.. IconProvider.Icons, .. iconStore.Icons];

    [RelayCommand]
    private void TogglePasswordReveal()
        => IsPasswordRevealed = !IsPasswordRevealed;

    [RelayCommand]
    private void SelectIcon(string icon)
    {
        SelectedIcon = icon;
        IsIconPickerOpen = false;
        ImportError = null;
        ImportSuccess = null;
    }

    [RelayCommand]
    private async Task ImportIcon()
    {
        ImportError = null;
        ImportSuccess = null;

        // Null result: the user cancelled the dialog
        var result = await iconImporter.ImportAsync();
        if (result is null)
            return;

        if (!result.IsSuccess)
        {
            ImportError = result.Message;
            return;
        }

        ImportSuccess = result.Message;
        if (result.Icon is { } icon)
        {
            if (!AvailableIcons.Contains(icon))
                AvailableIcons.Add(icon);
            SelectedIcon = icon;
        }
    }

    private bool CanSave()
        => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(PasswordValue);

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
        passwordsContainer.Add(new Password
        {
            Platform = new PlatformInfo { Name = Name, Icon = SelectedIcon ?? string.Empty },
            Value = PasswordValue
        });

        navigator.GoBackward();
    }

    [RelayCommand]
    private void Close()
        => navigator.GoBackward();
}
