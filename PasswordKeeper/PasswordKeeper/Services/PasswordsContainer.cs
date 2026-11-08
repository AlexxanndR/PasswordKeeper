using PasswordKeeper.Models.Password;
using PasswordKeeper.Navigation;
using PasswordKeeper.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace PasswordKeeper.Services;
public class PasswordsContainer(INavigator navigator)
{
    private ObservableCollection<PasswordViewModel> _passwords = [];
     
    public ObservableCollection<PasswordViewModel> Passwords => _passwords;

}
