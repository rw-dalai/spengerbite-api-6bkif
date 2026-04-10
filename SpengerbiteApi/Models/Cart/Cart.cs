using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Models;

public class Cart : EntityBase
{
    public Customer Customer { get; set; }

    public Restaurant Restaurant { get; set; }

    public List<CartItem> CartItems { get; set; } = new();


    // EF Core
    protected Cart() { }

    // Business Ctor
    public Cart(Customer customer, Restaurant restaurant)
    {
        Customer = customer;
        Restaurant = restaurant;
    }
    

    public bool IsForRestaurant(int restaurantId) => 
        Restaurant.Id == restaurantId;

    // --- Story 4: Add item to cart ---
    // TODO: Find existing item by menu item id, increase quantity if found, add new if not
    //   If found: increase its Quantity
    //   If not found: add new CartItem(this, menuItem, quantity) to CartItems
    public void AddOrIncreaseItem(MenuItem menuItem, int quantity)
    {
        throw new NotImplementedException();
    }

    // --- Story 5: Clear cart ---
    // TODO: Clear all items from the cart
    public void ClearItems()
    {
        throw new NotImplementedException();
    }

    // --- Story 7: Place order ---
    // TODO: Return true if the cart has no items
    public bool IsEmpty => throw new NotImplementedException();
}
