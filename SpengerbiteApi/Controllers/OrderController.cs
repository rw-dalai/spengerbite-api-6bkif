using Microsoft.AspNetCore.Mvc;
using SpengerbiteApi.Services;
using SpengerbiteApi.ViewModels;

namespace SpengerbiteApi.Controllers;

[ApiController]
[Route("api/customers/{customerId}/orders")]
public class OrderController(
    IOrderService orderService
) : ControllerBase
{
    // GET /api/customers/{customerId}/orders -> 200 OK
    [HttpGet]
    public async Task<ActionResult<List<OrderSummaryResponse>>> GetOrdersAsync(int customerId)
    {
        var orders = await orderService.GetOrdersForCustomerAsync(customerId);
        return Ok(orders);
    }

    // ═══════════════════════════════════════════════════════════
    // Story 6: Cancel order
    // ═══════════════════════════════════════════════════════════
    // TODO: Add [HttpPost("{orderId}/cancel")] endpoint for cancelling an order
    //   Parameters: int customerId (from route), int orderId (from route)
    //   Return: NoContent() -> 204 No Content

    // ═══════════════════════════════════════════════════════════
    // Story 7: Place order
    // ═══════════════════════════════════════════════════════════
    // TODO: Add [HttpPost] endpoint for placing an order
    //   Parameter: int customerId (from route)
    //   Return: CreatedAtAction(nameof(GetOrdersAsync), ...) -> 201 Created
}
