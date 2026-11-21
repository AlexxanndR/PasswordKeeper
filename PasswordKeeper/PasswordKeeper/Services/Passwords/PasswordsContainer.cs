using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PasswordKeeper.Models.Password;
using PasswordKeeper.Navigation;
using PasswordKeeper.ViewModels;

namespace PasswordKeeper.Services.Passwords;

public class PasswordsContainer(INavigator navigator)
{
    private ObservableCollection<PasswordViewModel> _passwords = [];
    public ObservableCollection<PasswordViewModel> Passwords => _passwords;

    public void Add(Password password) =>
        _passwords.Add(new PasswordViewModel(navigator) { Password = password });

    public void Update(Password password)
    {
        var viewModel = _passwords.FirstOrDefault(vm => vm.Password.Id == password.Id);
        if (viewModel is not null)
            viewModel.Password = password;
    }

    public void Load(IEnumerable<Password> passwords)
    {
        var viewModels = passwords.Select(password => new PasswordViewModel(navigator)
        {
            Password = password,
        });
        _passwords = [.. viewModels];
    }
}
