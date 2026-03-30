using SpengerbiteApi.Models.Customer;

namespace SpengerbiteApi.Services;

public interface ICustomerService
{
    public Task<RegisteredCustomer?> GetCustomerAsync(int id);
    
    public Task<RegisteredCustomer> RegisterCustomerAsync(ViewModels.RegisterCustomerRequest request);
    
    public Task<RegisteredCustomer> UpdateCustomer(int id /* TODO */);
    
    public Task<RegisteredCustomer> ChangeCustomerName(int id /* TODO */);
    
    public Task DeleteCustomer(int id);
}