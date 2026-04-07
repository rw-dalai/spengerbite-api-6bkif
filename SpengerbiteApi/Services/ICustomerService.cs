using SpengerbiteApi.ViewModels;

namespace SpengerbiteApi.Services;

public interface ICustomerService
{
    Task<CustomerResponse> GetCustomerAsync(int customerId);
    
    Task<CustomerResponse> RegisterCustomerAsync(RegisterCustomerRequest request);
    
    Task<CustomerResponse> UpdateCustomerAsync(int customerId, UpdateCustomerRequest request);
    
    Task<CustomerResponse> ChangeCustomerNameAsync(int customerId, ChangeCustomerNameRequest request);
    
    Task DeleteCustomerAsync(int customerId);
}
