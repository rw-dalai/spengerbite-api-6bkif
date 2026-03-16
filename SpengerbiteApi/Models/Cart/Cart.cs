using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Models.Cart;

public class Cart : EntityBase
{
    public Customer.Customer Customer { get; set; }

    public Restaurant.Restaurant Restaurant { get; set; }

    public List<CartItem> CartItems { get; set; }


    // EF Core
    protected Cart() { }

    // Business Ctor
    public Cart(Customer.Customer customer, Restaurant.Restaurant restaurant)
    {
        Customer = customer;
        Restaurant = restaurant;
    }
}
