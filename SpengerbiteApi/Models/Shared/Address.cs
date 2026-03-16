namespace SpengerbiteApi.Models.Shared;

public record Address
{
    public string Street { get; init; }

    public string Zip { get; init; }

    public string City { get; init; }

    public string Country { get; init; }

    public Address(string street, string zip, string city, string country)
    {
        // TODO: validation
        Street = street;
        Zip = zip;
        City = city;
        Country = country;
    }
}
