using Microsoft.Extensions.DependencyInjection;
using PasswordKeeper.ViewModels;

namespace Mvvm.Navigation;
public static partial class ServiceCollectionExtensions
{
    public static void AddViewModels(this IServiceCollection collection)
    {
        collection.AddSingleton<MainViewModel>();
        collection.AddTransient<PasswordsListViewModel>();
        collection.AddTransient<PasswordManagerViewModel>();
    }
}
