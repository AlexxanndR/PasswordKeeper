using Microsoft.Extensions.DependencyInjection;
using PasswordKeeper.Navigation;
using PasswordKeeper.Services.Icons;
using PasswordKeeper.Services.Passwords;
using PasswordKeeper.ViewModels;

namespace Mvvm.Navigation;
public static partial class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<INavigator, Navigator>();
        collection.AddSingleton<PasswordsContainer>();
        collection.AddSingleton<IconStore>();
        collection.AddSingleton<IconImporter>();
    }

    public static void AddViewModels(this IServiceCollection collection)
    {
        collection.AddSingleton<MainViewModel>();
        collection.AddSingleton<PasswordsListViewModel>();
        collection.AddTransient<PasswordManagerViewModel>();
    }
}
