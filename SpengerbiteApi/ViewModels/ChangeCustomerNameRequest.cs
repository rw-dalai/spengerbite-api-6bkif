namespace SpengerbiteApi.ViewModels;

public record ChangeCustomerNameRequest
{
    public required string FirstName { get; init; }

    public required string LastName { get; init; }
}
