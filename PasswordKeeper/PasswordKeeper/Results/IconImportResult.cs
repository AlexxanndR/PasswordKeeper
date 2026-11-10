namespace PasswordKeeper.Results;

/// <summary>
/// Outcome of an icon import
/// </summary>
public sealed record IconImportResult : Result
{
    public string? Icon { get; private init; }

    public static IconImportResult Success(string icon, string message)
        => new() { IsSuccess = true, Icon = icon, Message = message };

    public static new IconImportResult Failure(string message)
        => new() { Message = message };
}
