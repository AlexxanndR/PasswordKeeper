using Avalonia.Controls.Notifications;
using PasswordKeeper.Results;

namespace PasswordKeeper.Services.Notifications;

/// <summary>
/// Toast helpers for <see cref="INotificationService"/>
/// </summary>
public static class NotificationExtensions
{
    /// <summary>
    /// Shows an operation result as a toast of the matching type
    /// </summary>
    public static void Show(this INotificationService service, Result result) =>
        service.Show(
            result.Message,
            result.IsSuccess ? NotificationType.Success : NotificationType.Error
        );
}
