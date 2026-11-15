using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace PasswordKeeper.Services.Database;

/// <summary>
/// Creates <see cref="VaultContext"/> instances against the unlocked vault.
/// The connection string is read lazily so the factory can be resolved
/// before the master password has been entered.
/// </summary>
public class VaultContextFactory(VaultManager vaultManager) : IDbContextFactory<VaultContext>
{
    public VaultContext CreateDbContext()
        => new(new DbContextOptionsBuilder<VaultContext>()
            .UseSqlite(vaultManager.ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options);

    public Task<VaultContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(CreateDbContext());
}
