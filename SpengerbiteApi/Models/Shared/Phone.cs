namespace SpengerbiteApi.Models.Shared;

public record Phone
{
    public string Value { get; init; }

    public Phone(string value)
    {
        // TODO: validation
        Value = value;
    }
}
