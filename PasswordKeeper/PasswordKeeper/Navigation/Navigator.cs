using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PasswordKeeper.Navigation;
public class Navigator(IServiceProvider serviceProvider) : INavigator
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    private readonly Stack<object> _history = new();

    private object? _currentViewModel;
    public object? CurrentViewModel
    {
        get => _currentViewModel;
        private set
        {
            if (_currentViewModel == value) return;

            var prevViewModel = _currentViewModel;
            _currentViewModel = value;

            NavigationChanged?.Invoke(this, new(prevViewModel, value, NavigationMode.NEW));
        }
    }

    public bool CanBackward => _history.Count > 0;
    
    public event EventHandler<NavigationEventArgs>? NavigationChanged;

    public void NavigateTo<TViewModel>() where TViewModel : class
        => Navigate(_serviceProvider.GetRequiredService<TViewModel>());

    public void NavigateTo<TViewModel, TParam>(params TParam[] parameters) where TViewModel : class
    {
        var viewModel = ActivatorUtilities.CreateInstance<TViewModel>(_serviceProvider, parameters);
        Navigate(viewModel);
    }

    private void Navigate(object viewModel)
    {
        if (_currentViewModel != null)
            _history.Push(_currentViewModel);
        CurrentViewModel = viewModel;
    }

    public async Task GoBackward()
    {
        if (!CanBackward) 
            return;

        var prevViewModel = _currentViewModel;
        _currentViewModel = _history.Pop();
        NavigationChanged?.Invoke(this, new(prevViewModel, _currentViewModel, NavigationMode.BACK));

        if (prevViewModel is IAsyncDisposable asyncDisposable) await asyncDisposable.DisposeAsync();
        else if (prevViewModel is IDisposable disposable) disposable.Dispose();
    }
}
