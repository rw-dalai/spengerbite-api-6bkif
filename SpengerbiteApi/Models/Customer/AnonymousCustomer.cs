namespace SpengerbiteApi.Models.Customer;

public class AnonymousCustomer : Customer
{
    public DateTime LastSeenAt { get; set; }


    // EF Core
    protected AnonymousCustomer() { }

    // Business Ctor
    public AnonymousCustomer(DateTime lastSeenAt)
    {
        LastSeenAt = lastSeenAt;
    }
}
