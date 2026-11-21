using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PasswordKeeper.Models.Password;
using PasswordKeeper.Navigation;
using PasswordKeeper.Services.Notifications;
using PasswordKeeper.ViewModels;

namespace PasswordKeeper.Services.Passwords;

public class PasswordsContainer(
    INavigator navigator,
    PasswordsRepository passwordsRepository,
    INotificationService notificationService
)
{
    private ObservableCollection<PasswordViewModel> _passwords = [];
    public ObservableCollection<PasswordViewModel> Passwords => _passwords;

    public void Add(Password password) => _passwords.Add(CreateViewModel(password));

    public void Update(Password password)
    {
        var viewModel = _passwords.FirstOrDefault(vm => vm.Password.Id == password.Id);
        if (viewModel is not null)
            viewModel.Password = password;
    }

    public void Remove(Password password)
    {
        var viewModel = _passwords.FirstOrDefault(vm => vm.Password.Id == password.Id);
        if (viewModel is not null)
            _passwords.Remove(viewModel);
    }

    public void Load(IEnumerable<Password> passwords)
    {
        var viewModels = passwords.Select(CreateViewModel);
        _passwords = [.. viewModels];
    }

    private PasswordViewModel CreateViewModel(Password password) =>
        new(navigator, this, passwordsRepository, notificationService) { Password = password };
}
