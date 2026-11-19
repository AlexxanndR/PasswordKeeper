using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using PasswordKeeper.Models.Password;
using PasswordKeeper.Navigation;
using PasswordKeeper.Services.Icons;
using PasswordKeeper.Services.Notifications;
using PasswordKeeper.Services.Passwords;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PasswordKeeper.ViewModels;

public partial class PasswordManagerViewModel(
    INavigator navigator,
    IconStore iconStore,
    IconImporter iconImporter,
    PasswordsContainer passwordsContainer,
    PasswordsRepository passwordsRepository,
    INotificationService notificationService,
    IValidator<PasswordManagerViewModel> validator,
    Password? password = null) : ValidatableViewModelBase<PasswordManagerViewModel>(validator)
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _name = password?.Platform.Name ?? string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _value = password?.Value ?? string.Empty;

    [ObservableProperty]
    private bool _isPasswordRevealed;

    [ObservableProperty]
    private bool _isIconPickerOpen;

    [ObservableProperty]
    private string? _selectedIcon = string.IsNullOrEmpty(password?.Platform.Icon)
        ? IconProvider.DefaultIcon
        : password.Platform.Icon;

    public ObservableCollection<string> AvailableIcons { get; } = [.. IconProvider.Icons, .. iconStore.Icons];

    [RelayCommand]
    private void TogglePasswordReveal()
        => IsPasswordRevealed = !IsPasswordRevealed;

    [RelayCommand]
    private void SelectIcon(string icon)
    {
        SelectedIcon = icon;
        IsIconPickerOpen = false;
    }

    [RelayCommand]
    private async Task ImportIcon()
    {
        // Null result: the user cancelled the dialog
        var result = await iconImporter.ImportAsync();

        if (result is not null)
            notificationService.Show(result.Message, result.IsSuccess ? NotificationType.Success : NotificationType.Error);

        if (result is { IsSuccess: true, Icon: { } icon })
        {
            if (!AvailableIcons.Contains(icon))
                AvailableIcons.Add(icon);
            SelectedIcon = icon;
        }
    }

    private bool CanSave() => IsValid;

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task Save()
    {
        if (password is { } editPassword)
            await SaveEdited(editPassword.Id);
        else
            await SaveNew();
    }

    private async Task SaveNew()
    {
        var password = new Password
        {
            Platform = new PlatformInfo { Name = Name, Icon = SelectedIcon ?? string.Empty },
            Value = Value
        };

        // Write to the encrypted vault first; only show it in the list once saved
        var result = await passwordsRepository.AddAsync(password);
        if (!result.IsSuccess)
        {
            notificationService.Show(result.Message, NotificationType.Error);
            return;
        }

        passwordsContainer.Add(password);
        await navigator.GoBackward();
    }

    private async Task SaveEdited(int passwordId)
    {
        var password = new Password
        {
            Id = passwordId,
            Platform = new PlatformInfo { Name = Name, Icon = SelectedIcon ?? string.Empty },
            Value = Value
        };

        // Write to the encrypted vault first; only refresh the list row once saved
        var result = await passwordsRepository.UpdateAsync(password);
        if (!result.IsSuccess)
        {
            notificationService.Show(result.Message, NotificationType.Error);
            return;
        }

        passwordsContainer.Update(password);
        await navigator.GoBackward();
    }

    [RelayCommand]
    private void Close()
        => navigator.GoBackward();
}
