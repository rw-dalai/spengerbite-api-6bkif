using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.ViewModels;

public record UpdateCustomerRequest
{
    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required Phone? Phone { get; init; }

    public required Address Address { get; init; }
}
