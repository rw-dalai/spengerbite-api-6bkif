namespace SpengerbiteApi.Models.Shared;

public record Email
{
    public string Value { get; init; }

    public Email(string value)
    {
        // TODO: validation
        Value = value;
    }
}
