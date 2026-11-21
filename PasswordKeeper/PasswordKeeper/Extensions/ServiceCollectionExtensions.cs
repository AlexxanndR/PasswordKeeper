using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PasswordKeeper.Navigation;
using PasswordKeeper.Services.Database;
using PasswordKeeper.Services.Icons;
using PasswordKeeper.Services.Notifications;
using PasswordKeeper.Services.Passwords;
using PasswordKeeper.Validation;
using PasswordKeeper.ViewModels;

namespace Mvvm.Navigation;

public static partial class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<INavigator, Navigator>();

        collection.AddSingleton<PasswordsContainer>();
        collection.AddSingleton<PasswordsRepository>();

        collection.AddSingleton<IconStore>();
        collection.AddSingleton<VaultManager>();

        collection.AddSingleton<NotificationService>();
        collection.AddSingleton<INotificationService>(sp =>
            sp.GetRequiredService<NotificationService>()
        );

        collection.AddSingleton<IValidator<UnlockViewModel>, UnlockViewModelValidator>();
        collection.AddSingleton<
            IValidator<PasswordManagerViewModel>,
            PasswordManagerViewModelValidator
        >();

        collection.AddSingleton<IconImporter>();
        collection.AddSingleton<IDbContextFactory<VaultContext>, VaultContextFactory>();
    }

    public static void AddViewModels(this IServiceCollection collection)
    {
        collection.AddSingleton<MainViewModel>();
        collection.AddSingleton<PasswordsListViewModel>();
        collection.AddTransient<PasswordManagerViewModel>();
        collection.AddTransient<UnlockViewModel>();
    }
}
