using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PasswordKeeper.Models.Password;
using PasswordKeeper.Results;
using PasswordKeeper.Services.Database;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PasswordKeeper.Services.Passwords;

/// <summary>
/// Data access for <see cref="Password"/> entries in the encrypted vault
/// </summary>
public class PasswordsRepository(IDbContextFactory<VaultContext> contextFactory)
{
    public async Task<IReadOnlyList<Password>> LoadAllAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Passwords.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Result> AddAsync(Password password, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            context.Passwords.Add(password);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success($"Saved \"{password.Platform.Name}\"");
        }
        catch (SqliteException ex)
        {
            return Result.Failure($"Could not save the password: {ex.Message}");
        }
    }
}
