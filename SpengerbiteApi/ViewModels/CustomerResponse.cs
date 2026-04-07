using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.ViewModels;

public record CustomerResponse(
    int Id,
    string FirstName,
    string LastName,
    Email? Email,
    Phone? Phone
);
