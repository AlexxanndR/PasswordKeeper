using PasswordKeeper.Models.Password;
using PasswordKeeper.Navigation;
using PasswordKeeper.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PasswordKeeper.Services.Passwords;

public class PasswordsContainer(INavigator navigator)
{
    private ObservableCollection<PasswordViewModel> _passwords = [];
    public ObservableCollection<PasswordViewModel> Passwords => _passwords;

    public void Add(Password password)
        => _passwords.Add(new PasswordViewModel(navigator) { Password = password });

    public void Load(IEnumerable<Password> passwords)
    {
        var viewModels = passwords
            .Select(password => new PasswordViewModel(navigator) { Password = password });
        _passwords = [with(viewModels)];
    }
}
