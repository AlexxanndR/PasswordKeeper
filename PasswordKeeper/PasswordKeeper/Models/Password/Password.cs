namespace PasswordKeeper.Models.Password;

public class Password
{
    public PlatformInfo Platform { get; set; } = new();

    public string Value { get; set; } = string.Empty;
}
