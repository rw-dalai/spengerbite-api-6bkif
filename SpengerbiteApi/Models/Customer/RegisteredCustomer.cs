using SpengerbiteApi.Models.Auth;
using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Models.Customer;

public class RegisteredCustomer : Customer
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public Phone? Phone { get; set; }

    public Address Address { get; set; }

    public UserAccount Account { get; set; }


    // EF Core
    public RegisteredCustomer() { }

    // Business Ctor
    public RegisteredCustomer(string firstName, string lastName, Phone? phone, Address address, UserAccount account)
    {
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Address = address;
        Account = account;
    }
}
