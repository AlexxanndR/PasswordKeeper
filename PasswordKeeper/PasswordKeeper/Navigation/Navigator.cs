using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace PasswordKeeper.Navigation;

public class Navigator(IServiceProvider serviceProvider) : INavigator
{
    private readonly Stack<object> _history = new();

    private object? _currentViewModel;
    public object? CurrentViewModel => _currentViewModel;

    public bool CanBackward => _history.Count > 0;

    public event EventHandler<NavigationEventArgs>? NavigationChanged;

    public void NavigateTo<TViewModel>()
        where TViewModel : class => Navigate(serviceProvider.GetRequiredService<TViewModel>());

    public void NavigateTo<TViewModel, TParam>(params TParam[] parameters)
        where TViewModel : class
        where TParam : class =>
        Navigate(ActivatorUtilities.CreateInstance<TViewModel>(serviceProvider, parameters));

    public async Task NavigateReplacing<TViewModel>()
        where TViewModel : class
    {
        var viewModel = serviceProvider.GetRequiredService<TViewModel>();
        if (ReferenceEquals(_currentViewModel, viewModel))
            return;

        var replaced = _currentViewModel;
        SetCurrent(viewModel, NavigationMode.REPLACE);
        await DisposeViewModelAsync(replaced);
    }

    public async Task GoBackward()
    {
        if (!CanBackward)
            return;

        var leaving = _currentViewModel;
        SetCurrent(_history.Pop(), NavigationMode.BACK);
        await DisposeViewModelAsync(leaving);
    }

    private void Navigate(object viewModel)
    {
        if (ReferenceEquals(_currentViewModel, viewModel))
            return;

        if (_currentViewModel is { } previous)
            _history.Push(previous);

        SetCurrent(viewModel, NavigationMode.NEW);
    }

    private void SetCurrent(object? viewModel, NavigationMode mode)
    {
        if (ReferenceEquals(_currentViewModel, viewModel))
            return;

        var previous = _currentViewModel;
        _currentViewModel = viewModel;

        NavigationChanged?.Invoke(this, new(previous, viewModel, mode));
    }

    private static async Task DisposeViewModelAsync(object? viewModel)
    {
        if (viewModel is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync();
        else if (viewModel is IDisposable disposable)
            disposable.Dispose();
    }
}
