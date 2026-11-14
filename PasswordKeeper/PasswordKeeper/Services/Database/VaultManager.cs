using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PasswordKeeper.Results;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PasswordKeeper.Services.Database;

/// <summary>
/// Turns a master password into the SQLCipher key for the vault and owns the
/// database connection string. <see cref="ConnectionString"/> is only valid
/// after <see cref="UnlockAsync"/> succeeds.
/// </summary>
public class VaultManager
{
    private const int KeySizeBytes = 32;
    private const int Pbkdf2Iterations = 200_000;

    private string? _connectionString;
    public string ConnectionString =>
        _connectionString ?? throw new InvalidOperationException("The vault has not been unlocked");

    public string VaultDirectory { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PasswordKeeper");

    public string DatabasePath => Path.Combine(VaultDirectory, "passwords.db");
    public string SaltPath => Path.Combine(VaultDirectory, "vault.salt");

    public bool IsFirstRun => !File.Exists(DatabasePath);

    public async Task<VaultUnlockResult> UnlockAsync(string masterPassword)
    {
        var exist = !IsFirstRun;

        try
        {
            Directory.CreateDirectory(VaultDirectory);

            var key = DeriveKey(masterPassword, GetOrCreateSalt());
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = DatabasePath,
                Password = $"x'{Convert.ToHexString(key)}'"
            };
            _connectionString = builder.ToString();

            await using var probe = new VaultContext(
                new DbContextOptionsBuilder<VaultContext>()
                    .UseSqlite(_connectionString)
                    .UseSnakeCaseNamingConvention()
                    .Options);

            // Opens the database with the key: a wrong key fails here as "file is not a database"
            await probe.Database.EnsureCreatedAsync();

            return VaultUnlockResult.Success(exist ? "Vault unlocked" : "Vault created");
        }
        catch (SqliteException)
        {
            _connectionString = null;
            return VaultUnlockResult.Failure(exist
                ? "Incorrect master password"
                : "Could not create the vault database");
        }
    }

    private byte[] GetOrCreateSalt()
    {
        if (File.Exists(SaltPath))
            return File.ReadAllBytes(SaltPath);

        var salt = RandomNumberGenerator.GetBytes(KeySizeBytes / 2);
        File.WriteAllBytes(SaltPath, salt);

        return salt;
    }

    private static byte[] DeriveKey(string masterPassword, byte[] salt)
        => Rfc2898DeriveBytes.Pbkdf2(masterPassword, salt, Pbkdf2Iterations, HashAlgorithmName.SHA256, KeySizeBytes);
}
