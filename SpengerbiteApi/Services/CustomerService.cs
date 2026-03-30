using Microsoft.EntityFrameworkCore;
using SpengerbiteApi.Infrastructure;
using SpengerbiteApi.Models.Auth;
using SpengerbiteApi.Models.Customer;
using SpengerbiteApi.Models.Shared;
using SpengerbiteApi.ViewModels;

namespace SpengerbiteApi.Services;

// scoped
public class CustomerService(
    SpengerbiteContext db, // scoped
    ILogger<CustomerService> logger
) : ICustomerService
{
    public async Task<RegisteredCustomer?> GetCustomerAsync(int id)
    {
        // Ingress Log
        logger.LogDebug("Fetching customer with id {CustomerId}", id);
        
        RegisteredCustomer? customer = await db.RegisteredCustomers
            .Include(rc => rc.Account)
            .FirstOrDefaultAsync(rc => rc.Id == id);
        
        // Egress Log
        logger.LogInformation("Fetched customer with id {CustomerId}", id);

        return customer;
    }

    public async Task<RegisteredCustomer> RegisterCustomerAsync(RegisterCustomerRequest request)
    {
        // Ingress Log
        logger.LogDebug("Registering new customer with email {Email}", request.Email);
        
        // TODO Password hashing
        
        var account = new UserAccount(
            email: request.Email,
            passwordHash: new PasswordHash("password"),
            role: UserRole.Customer,
            isEnabled: true
        );
        
        var customer = new RegisteredCustomer(
            request.FirstName,
            request.LastName,
            phone: new Phone("1234567"),
            address: request.Address,
            account: account
        );
        
        db.RegisteredCustomers.Add(customer);
        await db.SaveChangesAsync();
        
        // Egress Log
        logger.LogInformation("Registered new customer with email {Email}", request.Email);

        // TODO Never return a domain model, always return ViewModel (RegisteredCustomerResponse)
        return customer;
    }

    public Task<RegisteredCustomer> UpdateCustomer(int id)
    {
        throw new NotImplementedException();
    }

    public Task<RegisteredCustomer> ChangeCustomerName(int id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCustomer(int id)
    {
        throw new NotImplementedException();
    }
}