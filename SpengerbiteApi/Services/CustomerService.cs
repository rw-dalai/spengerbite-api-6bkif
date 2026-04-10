// ══════════════════════════════════════════════════════════════
//  Service Layer — Quick Reference
// ══════════════════════════════════════════════════════════════
//
//  Why not put business logic in the controller?
//    Controllers handle HTTP concerns (routing, status codes, request/response).
//    Services handle business concerns (validation, rules, data access).
//    Separation makes code testable, reusable, and easier to understand.
//
// ──────────────────────────────────────────────────────────────
//  Responsibilities of a Service
// ──────────────────────────────────────────────────────────────
//
//  1. Receive a Request DTO (or primitive params) from the controller
//  2. Validate business rules (e.g. customer exists, cart not empty)
//  3. Load/create/update entities via EF Core (DbContext)
//  4. Call domain methods on entities (e.g. cart.ClearItems(), order.Cancel())
//  5. Save changes to database (await db.SaveChangesAsync())
//  6. Return a Response DTO to the controller (never return entities!)
//
// ──────────────────────────────────────────────────────────────
//  Exception Handling
// ──────────────────────────────────────────────────────────────
//
//  The service throws ServiceException for application outcomes:
//    ServiceException.NotFound("Customer not found") -> 404 NOT FOUND
//    ServiceException.Forbidden("Order does not belong to customer") -> 403 FORBIDDEN
//    ServiceException.Conflict("Cart is empty") -> 409 CONFLICT
//
// ──────────────────────────────────────────────────────────────
//  Typical Service Method Structure
// ──────────────────────────────────────────────────────────────
//
//  public async Task<OrderResponse> PlaceOrderAsync(int customerId)
//  {
//      logger.LogDebug("Placing order for customer {CustomerId}", customerId);
//
//      // 1. Load entities (-> 404 NOT FOUND)
//      var customer = await db.Customers.FindAsync(customerId)
//          ?? throw ServiceException.NotFound($"Customer not found: {customerId});
//
//      var cart = await db.Carts.Include(c => c.CartItems)
//          .FirstOrDefaultAsync(c => c.Customer.Id == customerId)
//          ?? throw ServiceException.NotFound($"Cart not found for customer: {customerId});
//
//      // 2. Business rules (-> 409 CONFLICT)
//      if (cart.IsEmpty)
//         throw ServiceException.Conflict("Cart is empty");
//
//      // 3. Domain logic (may throw ArgumentException/DomainException -> 400 BAD REQUEST)
//      var order = new Order(...);
//      order.AddLinesFromCart(cart);
//      cart.ClearItems();
//
//      // 4. Save
//      db.Orders.Add(order);
//      await db.SaveChangesAsync(); // (DbUpdateException -> 409 CONFLICT if constraint violation)
//
//      // 5. Entity to DTO mapping
//      var response = new OrderResponse(order.Id, ...);
//
//      logger.LogInformation("Placed order {OrderId} for customer {CustomerId}", order.Id, customerId);
//      return response;
//  }
//
// ══════════════════════════════════════════════════════════════
//  LINQ — Quick Reference
// ══════════════════════════════════════════════════════════════
//
//  LINQ (Language Integrated Query) lets you query data sources with a consistent syntax,
//  whether the source is a List<T>, an array, or a database (via EF Core).
//
// ──────────────────────────────────────────────────────────────
//  Filtering (Where / FirstOrDefault)
// ──────────────────────────────────────────────────────────────
//  .Where(o => o.Status != OrderStatus.Cancelled)      Filter by condition
//  .FirstOrDefaultAsync(c => c.Id == id)               Get single element by condition (or null if not found)
//
// ──────────────────────────────────────────────────────────────
//  Projection (Select)
// ──────────────────────────────────────────────────────────────
//  .Select(c => c.FirstName)                           Extract one property
//  .Select(c => new CustomerResponse(c.Id, ...))       Project to DTO
//  .Select(c => new { c.Id, c.FirstName })             Project to anonymous type
//
// ──────────────────────────────────────────────────────────────
//  Sorting
// ──────────────────────────────────────────────────────────────
//  .OrderBy(o => o.OrderedAt)                          Sort ascending
//  .OrderByDescending(o => o.OrderedAt)                Sort descending
//
// ──────────────────────────────────────────────────────────────
//  Aggregation
// ──────────────────────────────────────────────────────────────
//  .Count()                                            Number of elements
//  .Sum(oi => oi.Price * oi.Quantity)                  Sum of computed values
//
// ──────────────────────────────────────────────────────────────
//  Materialization
// ──────────────────────────────────────────────────────────────
//  .ToListAsync()                                      Execute query and collect into a List<T>
//
// ──────────────────────────────────────────────────────────────
//  Existence Checks
// ──────────────────────────────────────────────────────────────
//  .Any(o => o.Status == OrderStatus.Submitted)        True if at least ONE matches
//  .All(o => o.Status == OrderStatus.Delivered)        True if ALL match
//
// ──────────────────────────────────────────────────────────────
//  Nesting (Any/Count/Sum/Select/SelectMany on list navigation properties)
// ──────────────────────────────────────────────────────────────
//  // Get all restaurants that have at least one submitted order
//  db.Restaurants.Where(r => r.Orders.Any(o => o.Status == ...))
//
//  // Get all restaurants with the count of their menu items
//  db.Restaurants.Select(r => new { r.Name, Count = r.MenuItems.Count })
//
//  // Get all restaurants with their menu items as DTOs
//  db.Restaurants.Select(r => new (r.Id, r.Name,
//      r.MenuItems.Select(mi => new MenuItemDto(...)).ToList()))
//
//  // Get total revenue across all orders (flatten + aggregate)
//  db.Orders.SelectMany(o => o.OrderItems).Sum(oi => oi.Price * oi.Quantity)
//
// ══════════════════════════════════════════════════════════════

using Microsoft.EntityFrameworkCore;
using SpengerbiteApi.Infrastructure;
using SpengerbiteApi.Models;
using SpengerbiteApi.Models.Shared;
using SpengerbiteApi.ViewModels;
using SpengerbiteApi.Exceptions;

namespace SpengerbiteApi.Services;

public class CustomerService(
    SpengerbiteContext db,
    ILogger<CustomerService> logger,
    IPasswordService passwordService
) : ICustomerService
{
    // Two ways to create a DTO from an entity:

    // V1: .Select(): usually for READ operations
    //     Projects directly in SQL, only fetches needed columns
    //     No .Include() needed, EF Core generates a targeted SELECT with JOIN

    // V2: Entity to DTO mapping: usually for WRITE operations
    //     Entity is already in memory after save, just construct the DTO
    
    // V1: Preferred for READ operations:
    // Loads only needed columns (no PasswordHash, Address, Role, ...)
    public async Task<CustomerResponse> GetCustomerAsync(int customerId)
    {
        logger.LogDebug("Fetching customer with id {CustomerId}", customerId);

        // Load and project to DTO (NotFound -> 404)
        var customerResponse = await db.RegisteredCustomers
            .Where(c => c.Id == customerId)
            .Select(c => new CustomerResponse(c.Id, c.FirstName, c.LastName, c.Account.Email, c.Phone))
            .FirstOrDefaultAsync()
            ?? throw ServiceException.NotFound($"Customer not found: {customerId}");

        logger.LogInformation("Fetched customer with id {CustomerId}", customerId);
        return customerResponse;
    }

    // V2: Not preferred for READ operations
    // Loads the FULL entity with ALL columns (PasswordHash, Address, Role, ...)
    // public async Task<CustomerResponse> GetCustomerAsync(int customerId)
    // {
    //     var customer = await db.RegisteredCustomers
    //         .Include(rc => rc.Account)
    //         .FirstOrDefaultAsync(rc => rc.Id == customerId)
    //         ?? throw ServiceException.NotFound($"Customer not found: {customerId}");
    //
    //     return new CustomerResponse(
    //         customer.Id,
    //         customer.FirstName,
    //         customer.LastName,
    //         customer.Account.Email,
    //         customer.Phone);
    // }

    public async Task<CustomerResponse> RegisterCustomerAsync(RegisterCustomerRequest request)
    {
        logger.LogDebug("Registering new customer with email {Email}", request.Email);

        // Check email uniqueness (Conflict -> 409)
        if (await db.UserAccounts.AnyAsync(a => a.Email == request.Email))
            throw ServiceException.Conflict($"A customer with this email already exists: {request.Email}");

        // Hash password
        var hashedPassword = passwordService.HashPassword(request.Password);

        // Create entities
        var account = new UserAccount(
            email: request.Email,
            passwordHash: hashedPassword,
            role: UserRole.Customer,
            isEnabled: true
        );

        var customer = new RegisteredCustomer(
            firstName: request.FirstName,
            lastName: request.LastName,
            phone: request.Phone,
            address: request.Address,
            account: account
        );

        // Save to DB
        db.RegisteredCustomers.Add(customer);
        await db.SaveChangesAsync();

        // Map to DTO
        var customerResponse = new CustomerResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Account.Email,
            customer.Phone);
        
        logger.LogInformation("Registered new customer with email {Email}", request.Email);
        return customerResponse;
    }

    public async Task<CustomerResponse> UpdateCustomerAsync(int customerId, UpdateCustomerRequest request)
    {
        logger.LogDebug("Updating customer {CustomerId}", customerId);

        // Load entity (NotFound -> 404)
        var customer = await db.RegisteredCustomers
            .Include(rc => rc.Account)
            .FirstOrDefaultAsync(rc => rc.Id == customerId)
            ?? throw ServiceException.NotFound($"Customer not found: {customerId}");

        // Update and save
        customer.Update(request.FirstName, request.LastName, request.Phone, request.Address);
        await db.SaveChangesAsync();

        // Map to DTO
        var customerResponse = new CustomerResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Account.Email,
            customer.Phone);

        logger.LogInformation("Updated customer {CustomerId}", customerId);
        return customerResponse;
    }

    public async Task<CustomerResponse> ChangeCustomerNameAsync(int customerId, ChangeCustomerNameRequest request)
    {
        logger.LogDebug("Changing name for customer {CustomerId}", customerId);

        // Load entity (NotFound -> 404)
        var customer = await db.RegisteredCustomers
            .Include(rc => rc.Account)
            .FirstOrDefaultAsync(rc => rc.Id == customerId)
            ?? throw ServiceException.NotFound($"Customer not found: {customerId}");

        // Update and save
        customer.ChangeName(request.FirstName, request.LastName);
        await db.SaveChangesAsync();

        // Map to DTO
        var customerResponse = new CustomerResponse(
            customer.Id, customer.FirstName, customer.LastName,
            customer.Account.Email, customer.Phone);

        logger.LogInformation("Changed name for customer {CustomerId}", customerId);
        return customerResponse;
    }

    public async Task DeleteCustomerAsync(int customerId)
    {
        logger.LogDebug("Deleting customer {CustomerId}", customerId);

        // Load entity (NotFound -> 404)
        var customer = await db.RegisteredCustomers.FindAsync(customerId)
            ?? throw ServiceException.NotFound($"Customer not found: {customerId}");

        // Remove and save
        db.RegisteredCustomers.Remove(customer);
        await db.SaveChangesAsync();

        logger.LogInformation("Deleted customer {CustomerId}", customerId);
    }
}
