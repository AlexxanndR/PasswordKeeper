using Microsoft.EntityFrameworkCore;
using PasswordKeeper.Models.Password;

namespace PasswordKeeper.Services.Database;

/// <summary>
/// SQLCipher-backed store for the local passwords vault
/// </summary>
public class VaultContext(DbContextOptions<VaultContext> options) : DbContext(options)
{
    public DbSet<Password> Passwords => Set<Password>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Password>(entity =>
        {
            entity.HasKey(password => password.Id);
            entity.Property(password => password.Value).IsRequired();

            entity.OwnsOne(password => password.Platform, platform =>
            {
                platform.Property(info => info.Name).IsRequired();
            });
        });
    }
}
