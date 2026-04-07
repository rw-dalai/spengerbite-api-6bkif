using Microsoft.EntityFrameworkCore;
using SpengerbiteApi.Infrastructure;
using SpengerbiteApi.Models.Shared;
using SpengerbiteApi.ViewModels;
using SpengerbiteApi.Exceptions;

namespace SpengerbiteApi.Services;

public class OrderService(
    SpengerbiteContext db,
    ILogger<OrderService> logger
) : IOrderService
{
    public async Task<List<OrderSummaryResponse>> GetOrdersForCustomerAsync(int customerId)
    {
        logger.LogDebug("Loading orders for customerId={CustomerId}", customerId);

        // Check if customer exists (NotFound -> 404)
        if (!await db.RegisteredCustomers.AnyAsync(c => c.Id == customerId))
            throw ServiceException.NotFound($"Customer not found: {customerId}");

        // Load orders, filter, sort, project to DTO
        var summaries = await db.Orders
            .Where(o => o.Customer.Id == customerId)       // filter by customer
            .Where(o => o.Status != OrderStatus.Cancelled) // filter out cancelled
            .OrderByDescending(o => o.OrderedAt)           // order by newest first
            .Select(o => new OrderSummaryResponse(         // project to DTO
                o.Id,
                o.Restaurant.Name,
                o.Status.ToString(),
                o.OrderItems.Count,
                o.OrderItems.Sum(i => i.Price * i.Quantity)
            ))
            .ToListAsync();

        logger.LogInformation("Loaded {Count} orders for customerId={CustomerId}", summaries.Count, customerId);
        return summaries;
    }

    // ═══════════════════════════════════════════════════════════
    // Story 6: Cancel order
    // ═══════════════════════════════════════════════════════════
    // TODO Step 1: Load the customer (throw not found if missing)
    // TODO Step 2: Load the order with its customer (throw not found if missing)
    //   Use: await db.Orders.Include(..).FirstOrDefaultAsync(..)
    // TODO Step 3: Verify the order belongs to the customer (throw forbidden if not)
    // TODO Step 4: Cancel the order (may throw DomainException if already delivered)
    // TODO Step 5: Save and log

    // ═══════════════════════════════════════════════════════════
    // Story 7: Place order (depends on Story 5)
    // ═══════════════════════════════════════════════════════════
    // TODO Step 1: Load the customer (throw not found if missing)
    // TODO Step 2: Load the cart with items and menu items (throw not found if missing)
    //   Use: await db.Carts.Include(..).ThenInclude(..).FirstOrDefaultAsync(..)
    // TODO Step 3: Validate the cart is not empty (throw conflict if empty)
    // TODO Step 4: Create an order, copy lines from cart, clear cart, save
    // TODO Step 5: Return the mapped response
}
