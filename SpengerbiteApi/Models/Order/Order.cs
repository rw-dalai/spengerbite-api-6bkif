using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Models;

public class Order : EntityBase
{
    public DateTime OrderedAt { get; set; }

    public OrderStatus Status { get; set; }

    public RegisteredCustomer Customer { get; set; }

    public Restaurant Restaurant { get; set; }

    public List<OrderItem> OrderItems { get; set; } = new();


    // EF Core
    protected Order() { }

    // Business Ctor
    public Order(DateTime orderedAt, OrderStatus status, RegisteredCustomer customer, Restaurant restaurant)
    {
        OrderedAt = orderedAt;
        Status = status;
        Customer = customer;
        Restaurant = restaurant;
    }

    public decimal Total => 0; /* TODO Total Sum*/

    // --- Story 6: Cancel order ---
    // TODO: Throw DomainException if status is Delivered, otherwise set to Cancelled
    //   If Status == OrderStatus.Delivered -> throw new DomainException("Cannot cancel a delivered order")
    //   Otherwise -> Status = OrderStatus.Cancelled
    public void Cancel()
    {
        throw new NotImplementedException();
    }

    // --- Story 7: Place order ---
    // TODO: Copy each cart item into an OrderItem
    //   Create new OrderItem(this, cartItem.MenuItem, cartItem.Quantity, cartItem.MenuItem.Price)
    //   Add each to OrderItems
    public void AddLinesFromCart(Cart cart)
    {
        throw new NotImplementedException();
    }
}
