using SpengerbiteApi.Models.Customer;
using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Models.Order;

public class Order : EntityBase
{
    public DateTime OrderedAt { get; set; }

    public OrderStatus Status { get; set; }

    public RegisteredCustomer Customer { get; set; }

    public Restaurant.Restaurant Restaurant { get; set; }

    public List<OrderItem> OrderItems { get; set; }


    // EF Core
    protected Order() { }

    // Business Ctor
    public Order(DateTime orderedAt, OrderStatus status, RegisteredCustomer customer, Restaurant.Restaurant restaurant)
    {
        OrderedAt = orderedAt;
        Status = status;
        Customer = customer;
        Restaurant = restaurant;
    }
}
