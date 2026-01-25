using CommunityToolkit.Mvvm.Input;
using PasswordKeeper.Navigation;

namespace PasswordKeeper.ViewModels;
public partial class PasswordManagerViewModel(INavigator navigator) : ViewModelBase
{
    [RelayCommand]
    private void Close()
        => navigator.GoBackward();
}
