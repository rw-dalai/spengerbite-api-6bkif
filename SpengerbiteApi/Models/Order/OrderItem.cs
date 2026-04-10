using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Models;

public class OrderItem : EntityBase
{
    public Order Order { get; set; }

    public MenuItem MenuItem { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }


    // EF Core
    protected OrderItem() { }

    // Business Ctor
    public OrderItem(Order order, MenuItem menuItem, int quantity, decimal price)
    {
        Order = order;
        MenuItem = menuItem;
        Quantity = quantity;
        Price = price;
    }
}
