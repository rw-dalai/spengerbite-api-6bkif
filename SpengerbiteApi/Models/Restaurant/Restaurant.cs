using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Models.Restaurant;

public class Restaurant : EntityBase
{
    public string Name { get; set; }

    public Address Address { get; set; }

    public Phone Phone { get; set; }

    public OpeningHours OpeningHours { get; set; }

    public List<MenuItem> MenuItems { get; set; }


    // EF Core
    protected Restaurant() { }

    // Business Ctor
    public Restaurant(string name, Address address, Phone phone, OpeningHours openingHours)
    {
        Name = name;
        Address = address;
        Phone = phone;
        OpeningHours = openingHours;
    }
}
