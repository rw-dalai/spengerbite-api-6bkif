using SpengerbiteApi.Models;
using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Test;

// What is a Fixtures?
// A fixture is a fixed state of a set of objects
public static class Fixtures
{
    public static RegisteredCustomer NewRegisteredCustomer(
        string firstName = "Helene",
        string lastName = "Fischer"
    )
    {
        var account = new UserAccount(
            email: new Email($"{firstName}.{lastName}@spengergasse.at"),
            passwordHash: new PasswordHash("hashed_password_for_testing"),
            role: UserRole.Customer,
            isEnabled: true
        );

        return new RegisteredCustomer(
            firstName,
            lastName,
            phone: new Phone("1234567"),
            address: new Address("Spengergasse 20", "1050", "Wien", "AT"),
            account: account
        );
    }

    public static Restaurant NewRestaurant(string name = "Pasta Palace")
    {
        return new Restaurant(
            name,
            new Address("Mariahilfer Str. 1", "1060", "Wien", "AT"),
            new Phone("9876543"),
            new OpeningHours(new TimeOnly(10, 0), new TimeOnly(22, 0))
        );
    }

    public static MenuItem NewMenuItem(Restaurant restaurant, string name = "Spaghetti", decimal price = 8.50m)
    {
        return new MenuItem(name, "Delicious " + name, price, restaurant);
    }

    public static Order NewOrder(
        RegisteredCustomer customer,
        Restaurant restaurant,
        OrderStatus status = OrderStatus.Submitted)
    {
        return new Order(DateTime.Now, status, customer, restaurant);
    }
}
