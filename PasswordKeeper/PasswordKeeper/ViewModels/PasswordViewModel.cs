using CommunityToolkit.Mvvm.ComponentModel;

namespace PasswordKeeper.ViewModels;
public partial class PasswordViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _icon;

    [ObservableProperty]
    private string _name;
}
