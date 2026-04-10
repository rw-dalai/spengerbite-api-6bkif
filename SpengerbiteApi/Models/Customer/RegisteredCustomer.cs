using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Models;

public class RegisteredCustomer : Customer
{
    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public Phone? Phone { get; private set; }

    public Address Address { get; private set; }

    public UserAccount Account { get; private set; }


    // EF Core
    protected RegisteredCustomer() { }

    // Business Ctor
    public RegisteredCustomer(
        string firstName,
        string lastName,
        Phone? phone,
        Address address,
        UserAccount account)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(firstName);
        // TODO more validation
        
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Address = address;
        Account = account;
    }
    
    // --- Business methods ---

    public void Update(string firstName, string lastName, Phone? phone, Address address)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(lastName);
        // TODO more validation

        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Address = address;
    }

    public void ChangeName(string firstName, string lastName)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(lastName);
        // TODO more validation

        FirstName = firstName;
        LastName = lastName;
    }
}
