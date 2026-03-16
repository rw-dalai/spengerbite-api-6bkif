using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Models.Cart;

public class CartItem : EntityBase
{
    public Cart Cart { get; set; }

    public Restaurant.MenuItem MenuItem { get; set; }

    public int Quantity { get; set; }

    public string? Comment { get; set; }


    // EF Core
    protected CartItem() { }

    // Business Ctor
    public CartItem(Cart cart, Restaurant.MenuItem menuItem, int quantity, string? comment = null)
    {
        Cart = cart;
        MenuItem = menuItem;
        Quantity = quantity;
        Comment = comment;
    }
}
