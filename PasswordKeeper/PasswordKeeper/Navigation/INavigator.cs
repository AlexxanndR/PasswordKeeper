using System;
using System.Threading.Tasks;

namespace PasswordKeeper.Navigation;

public interface INavigator
{
    object? CurrentViewModel { get; }
    
    event EventHandler<NavigationEventArgs>? NavigationChanged;
    
    bool CanBackward { get; }
    Task GoBackward();

    void NavigateTo<TViewModel>() where TViewModel : class;
    void NavigateTo<TViewModel, TParam>(params TParam[] parameter) where TViewModel : class;

}
