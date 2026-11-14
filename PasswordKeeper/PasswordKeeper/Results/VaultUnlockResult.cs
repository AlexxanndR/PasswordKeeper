namespace PasswordKeeper.Results;

/// <summary>
/// Outcome of unlocking the encrypted vault
/// </summary>
public sealed record VaultUnlockResult : Result
{
    public static new VaultUnlockResult Success(string message)
        => new() { IsSuccess = true, Message = message };

    public static new VaultUnlockResult Failure(string message)
        => new() { Message = message };
}
