using SpengerbiteApi.Models.Auth;
using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Models.RestaurantOwner;

public class RestaurantOwner : EntityBase
{
    public string CompanyName { get; set; }

    public Address CompanyAddress { get; set; }

    public string VatId { get; set; }

    public Phone Phone { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public UserAccount Account { get; set; }


    // EF Core
    protected RestaurantOwner() { }

    // Business Ctor
    public RestaurantOwner(string companyName, Address companyAddress, string vatId, Phone phone, string firstName, string lastName, UserAccount account)
    {
        CompanyName = companyName;
        CompanyAddress = companyAddress;
        VatId = vatId;
        Phone = phone;
        FirstName = firstName;
        LastName = lastName;
        Account = account;
    }
}
