using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PasswordKeeper.Models.Password;
using PasswordKeeper.Results;
using PasswordKeeper.Services.Database;

namespace PasswordKeeper.Services.Passwords;

/// <summary>
/// Data access for <see cref="Password"/> entries in the encrypted vault
/// </summary>
public class PasswordsRepository(IDbContextFactory<VaultContext> contextFactory)
{
    private static readonly VaultOperation Add = new(
        static (context, entry) => context.Passwords.Add(entry),
        static password => $"Saved \"{password.Platform.Name}\"",
        static ex => $"Could not save the password: {ex.Message}"
    );

    private static readonly VaultOperation Update = new(
        static (context, entry) => context.Passwords.Update(entry),
        static password => $"Updated \"{password.Platform.Name}\"",
        static ex => $"Could not update the password: {ex.Message}"
    );

    private static readonly VaultOperation Remove = new(
        static (context, entry) => context.Passwords.Remove(entry),
        static password => $"Removed \"{password.Platform.Name}\"",
        static ex => $"Could not remove the password: {ex.Message}"
    );

    public async Task<IReadOnlyList<Password>> LoadAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Passwords.AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<Result> AddAsync(
        Password password,
        CancellationToken cancellationToken = default
    ) => ExecuteAsync(password, Add, cancellationToken);

    public Task<Result> UpdateAsync(
        Password password,
        CancellationToken cancellationToken = default
    ) => ExecuteAsync(password, Update, cancellationToken);

    public Task<Result> RemoveAsync(
        Password password,
        CancellationToken cancellationToken = default
    ) => ExecuteAsync(password, Remove, cancellationToken);

    private async Task<Result> ExecuteAsync(
        Password password,
        VaultOperation operation,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            operation.Apply(context, password);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success(operation.SuccessMessage(password));
        }
        catch (Exception ex) when (ex is SqliteException or DbUpdateException)
        {
            return Result.Failure(operation.FailureMessage(ex));
        }
    }

    /// <summary>
    /// A single vault operation: how to apply it and the messages describing its outcome
    /// </summary>
    private sealed record VaultOperation(
        Action<VaultContext, Password> Apply,
        Func<Password, string> SuccessMessage,
        Func<Exception, string> FailureMessage
    );
}
