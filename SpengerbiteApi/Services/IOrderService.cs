using SpengerbiteApi.ViewModels;

namespace SpengerbiteApi.Services;

public interface IOrderService
{
    Task<List<OrderSummaryResponse>> GetOrdersForCustomerAsync(int customerId);

    // Story 6: TODO add CancelOrderAsync method
    // Story 7: TODO add PlaceOrderAsync method
}
