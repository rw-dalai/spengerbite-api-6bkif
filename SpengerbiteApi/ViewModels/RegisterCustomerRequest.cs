using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.ViewModels;

public record RegisterCustomerRequest
{
    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required Address Address { get; init; }

    public required Email Email { get; init; }

    public required string Password { get; init; }
}