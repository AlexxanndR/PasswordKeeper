namespace PasswordKeeper.Results;

/// <summary>
/// Common outcome of an operation: a success flag plus a message
/// </summary>
public record Result
{
    public bool IsSuccess { get; protected init; }

    public string Message { get; protected init; } = string.Empty;

    public static Result Success(string message)
        => new() { IsSuccess = true, Message = message };

    public static Result Failure(string message)
        => new() { Message = message };
}
