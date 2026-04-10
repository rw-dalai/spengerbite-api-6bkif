using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Models;

public class MenuItem : EntityBase
{
    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public Restaurant Restaurant { get; set; }


    // EF Core
    protected MenuItem() { }

    // Business Ctor
    public MenuItem(string name, string description, decimal price, Restaurant restaurant)
    {
        Name = name;
        Description = description;
        Price = price;
        Restaurant = restaurant;
    }
}
