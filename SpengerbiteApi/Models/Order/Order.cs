using SpengerbiteApi.Models.Cart;
using SpengerbiteApi.Models.Customer;
using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Models.Order;

public class Order : EntityBase
{
    public DateTime OrderedAt { get; set; }

    public OrderStatus Status { get; set; }

    public RegisteredCustomer Customer { get; set; }

    public Restaurant.Restaurant Restaurant { get; set; }

    public List<OrderItem> OrderItems { get; set; } = new();


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

    public decimal Total => OrderItems?.Sum(i => i.Price * i.Quantity) ?? 0;

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
    //   Use: foreach over cart.CartItems
    //   Create new OrderItem(this, cartItem.MenuItem, cartItem.Quantity, cartItem.MenuItem.Price)
    //   Add each to OrderItems
    public void AddLinesFromCart(Cart.Cart cart)
    {
        throw new NotImplementedException();
    }
}
