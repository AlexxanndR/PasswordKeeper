using System;

namespace PasswordKeeper.Navigation;

public enum NavigationMode
{
    NEW,
    BACK,
    FORWARD
}

public class NavigationEventArgs(object? oldViewModel, object? newViewModel, NavigationMode mode) : EventArgs
{
    public object? OldViewModel { get; } = oldViewModel;
    public object? NewViewModel { get; } = newViewModel;
    public NavigationMode Mode { get; } = mode;
}
