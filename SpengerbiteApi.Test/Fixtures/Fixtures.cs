using SpengerbiteApi.Models.Auth;
using SpengerbiteApi.Models.Customer;
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
        // var account = new UserAccount(
            // email: new Email("helene.fischer@spengergasse.at"),
            // passwordHash: new PasswordHash("hashedpassword"),
            // role: UserRole.Customer,
            // isEnabled: true
        // );
        
        var account = new UserAccount(
            email: new Email($"{firstName}.{lastName}@spengergasse.at"),
            passwordHash: new PasswordHash("password"),
            role: UserRole.Customer,
            isEnabled: true
        );
        
        var customer = new RegisteredCustomer(
            firstName,
            lastName,
            phone: new Phone("1234567"),
            address: new Address(
                street: "Musterstraße 1",
                zip: "12345",
                city: "Musterstadt",
                country: "Deutschland"
            ),
            account: account
        );
        
        return customer;
    }
}