using PasswordKeeper.Models.Password;
using PasswordKeeper.Navigation;
using PasswordKeeper.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PasswordKeeper.Services.Passwords;
public class PasswordsContainer(INavigator navigator)
{
    private ObservableCollection<PasswordViewModel> _passwords = [];
    public ObservableCollection<PasswordViewModel> Passwords => _passwords;

    public void Add(Password password)
        => _passwords.Add(new PasswordViewModel(navigator) { Password = password });

    public void Load(IEnumerable<Password> passwords)
    {
        _passwords.Clear();
        foreach (var password in passwords)
            _passwords.Add(new PasswordViewModel(navigator) { Password = password });
    }
}
