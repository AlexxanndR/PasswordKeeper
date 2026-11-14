namespace PasswordKeeper.Models.Password;

public class Password
{
    public int Id { get; set; }

    public PlatformInfo Platform { get; set; } = new();

    public string Value { get; set; } = string.Empty;
}
