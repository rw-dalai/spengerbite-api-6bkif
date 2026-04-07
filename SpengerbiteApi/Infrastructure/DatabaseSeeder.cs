using Microsoft.AspNetCore.Identity;
using SpengerbiteApi.Models.Auth;
using SpengerbiteApi.Models.Cart;
using SpengerbiteApi.Models.Customer;
using SpengerbiteApi.Models.Order;
using SpengerbiteApi.Models.Restaurant;
using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Infrastructure;

public static class DatabaseSeeder
{
    public static void Seed(SpengerbiteContext db, PasswordHasher<UserAccount> passwordHasher)
    {
        if (db.RegisteredCustomers.Any()) return;

        // ── Customer ────────────────────────────────────────────
        var maxAccount = new UserAccount(
            new Email("max.mustermann@spengerbasse.at"),
            new PasswordHash(passwordHasher.HashPassword(null!, "Password1!")),
            UserRole.Customer,
            isEnabled: true);

        var max = new RegisteredCustomer(
            "Max",
            "Mustermann",
            new Phone("+43 660 1234567"),
            new Address("Spengergasse 20", "1050", "Wien", "AT"),
            maxAccount);

        db.RegisteredCustomers.Add(max);

        // ── Restaurants ─────────────────────────────────────────
        var pastaPalace = new Restaurant(
            "Pasta Palace",
            new Address("Mariahilfer Str. 1", "1060", "Wien", "AT"),
            new Phone("+43 1 1111111"),
            new OpeningHours(new TimeOnly(10, 0), new TimeOnly(22, 0)));

        var burgerBarn = new Restaurant(
            "Burger Barn",
            new Address("Wiedner Hauptstr. 42", "1040", "Wien", "AT"),
            new Phone("+43 1 2222222"),
            new OpeningHours(new TimeOnly(11, 0), new TimeOnly(23, 0)));

        db.Restaurants.AddRange(pastaPalace, burgerBarn);

        // ── Menu Items ──────────────────────────────────────────
        var spaghetti = new MenuItem("Spaghetti", "Classic tomato sauce", 8.50m, pastaPalace);
        var lasagna = new MenuItem("Lasagna", "Homemade beef lasagna", 12.90m, pastaPalace);
        var tiramisu = new MenuItem("Tiramisu", "Traditional Italian dessert", 6.50m, pastaPalace);

        var classicBurger = new MenuItem("Classic Burger", "Beef patty with lettuce and tomato", 9.90m, burgerBarn);
        var cheeseFries = new MenuItem("Cheese Fries", "Crispy fries with melted cheese", 5.50m, burgerBarn);

        db.MenuItems.AddRange(spaghetti, lasagna, tiramisu, classicBurger, cheeseFries);

        // ── Cart ────────────────────────────────────────────────
        var cart = new Cart(max, pastaPalace);
        db.Carts.Add(cart);

        var cartItem1 = new CartItem(cart, spaghetti, 2);
        var cartItem2 = new CartItem(cart, lasagna, 1);
        db.CartItems.AddRange(cartItem1, cartItem2);

        // ── Orders ──────────────────────────────────────────────
        // Submitted (today)
        var orderSubmitted = new Order(DateTime.Today, OrderStatus.Submitted, max, pastaPalace);
        db.Orders.Add(orderSubmitted);
        db.OrderItems.Add(new OrderItem(orderSubmitted, spaghetti, 1, 8.50m));

        // Delivered (yesterday)
        var orderDelivered = new Order(DateTime.Today.AddDays(-1), OrderStatus.Delivered, max, pastaPalace);
        db.Orders.Add(orderDelivered);
        db.OrderItems.Add(new OrderItem(orderDelivered, spaghetti, 1, 8.50m));

        // Cancelled (2 days ago)
        var orderCancelled = new Order(DateTime.Today.AddDays(-2), OrderStatus.Cancelled, max, pastaPalace);
        db.Orders.Add(orderCancelled);
        db.OrderItems.Add(new OrderItem(orderCancelled, spaghetti, 1, 8.50m));

        db.SaveChanges();
    }
}
