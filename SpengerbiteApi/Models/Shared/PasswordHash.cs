namespace SpengerbiteApi.Models.Shared;

public record PasswordHash
{
    public string Value { get; init; }

    public PasswordHash(string value)
    {
        // TODO: validation
        Value = value;
    }
}
