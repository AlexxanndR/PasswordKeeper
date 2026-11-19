using Avalonia.Controls.Notifications;

namespace PasswordKeeper.Services.Notifications;

/// <summary>
/// Shows transient, non-field-specific messages (toasts/snackbars).
/// </summary>
public interface INotificationService
{
    void Show(string message, NotificationType type = NotificationType.Information);
}
