using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;

namespace PasswordKeeper.Services.Notifications;

/// <summary>
/// Toasts via Avalonia's built-in notification system
/// </summary>
public sealed class NotificationService : INotificationService
{
    private TopLevel? _host;
    private WindowNotificationManager? _manager;

    public void Attach(TopLevel host)
    {
        // AttachedToVisualTree can fire again for the same host (e.g. mobile
        // re-attach): a second manager would stack duplicate toast layers
        if (ReferenceEquals(_host, host))
            return;

        _host = host;
        _manager = new WindowNotificationManager(host)
        {
            // Bottom-left: the bottom-right corner is reserved for FAB actions
            Position = NotificationPosition.BottomLeft,
            MaxItems = 3,
            Margin = new Thickness(16),
        };
    }

    public void Show(string message, NotificationType type = NotificationType.Information) =>
        _manager?.Show(new Notification(title: null, message, type));
}
