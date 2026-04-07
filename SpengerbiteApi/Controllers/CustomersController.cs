// ══════════════════════════════════════════════════════════════════════
//  RESTful API Design -- Quick Reference
// ══════════════════════════════════════════════════════════════════════
//
// - Each controller focuses on a single resource (e.g. customers, menu items, orders)
//
//  Verb     URL                  Action           Status Code
//  GET      /api/customers       List all         200 OK
//  GET      /api/customers/{id}  Get one          200 OK (or 404)
//  POST     /api/customers       Create           201 Created
//  PUT      /api/customers/{id}  Full update      200 OK (or 404)
//  PATCH    /api/customers/{id}  Partial update   200 OK (or 404)
//  DELETE   /api/customers/{id}  Delete           204 No Content
//
// ──────────────────────────────────────────────────────────────────────
//  Return Types
// ──────────────────────────────────────────────────────────────────────
//
//  - Use ActionResult<T> to returning either a successful response (e.g. Ok(T)), error response (e.g. NotFound())
//
//  Ok(...)                    -> 200 OK + JSON body
//  CreatedAtAction(...)       -> 201 Created + Location header + JSON body
//  NoContent()                -> 204 No Content (empty body)
//  NotFound()                 -> 404 Not Found
//  BadRequest(error)          -> 400 Bad Request
//  Conflict(error)            -> 409 Conflict

// ──────────────────────────────────────────────────────────────────────
//  Parameter Binding
// ──────────────────────────────────────────────────────────────────────
//
//  - ASP.NET Core automatically binds parameters from the URL path, query string, and request body.
//    But you can also use attributes [..] to specify where they come from.
//
//  [FromRoute] int id                           -> from URL path
//  [FromBody] RegisterCustomerRequest request  -> from JSON request body
//  [FromQuery] string? search                  -> from query string (?search=mustermann)
//
// ──────────────────────────────────────────────────────────────────────
//  Common Status Codes
// ──────────────────────────────────────────────────────────────────────
//
// - Use appropriate status codes to indicate the result of the operation.
//   This helps clients understand what happened and handle responses correctly.
//
//  Server Success (2xx):
//    200 OK                  -> successful GET, PUT, PATCH
//    201 Created             -> successful POST (resource created)
//    204 No Content          -> successful DELETE (or PUT/PATCH with no body)
//
// Client Errors (4xx):
//   400 Bad Request          -> invalid request (e.g. missing required field, invalid data type)
//   401 Unauthorized         -> not authenticated (e.g. missing token)
//   403 Forbidden            -> not authorized (e.g. trying to access another customer's data)
//   404 Not Found            -> resource not found (e.g. customer does not exist)
//   409 Conflict             -> business rule violation (e.g. email already taken)
//   422 Unprocessable Entity -> validation error (e.g. name must not be empty)

// Server Errors (5xx):
//   500 Internal Server Error -> unhandled exception (e.g. database down)
//
// ──────────────────────────────────────────────────────────────────────
// Problem Details (RFC 9457)
// ──────────────────────────────────────────────────────────────────────
//
// When an error occurs, we return a detailed JSON body with the following format (RFC 9457):
// {
//   "title": "Not Found",
//   "detail": "Customer not found: 123"
//   "status": 404,
//   ... (additional fields)
// }
//
// ──────────────────────────────────────────────────────────────────────
//  Example 1: Service throws exception -> GlobalExceptionHandler returns ProblemDetails
// ──────────────────────────────────────────────────────────────────────
//
//  // Controller: just delegates, no error handling needed
//  [HttpGet("{id}")]
//  public async Task<ActionResult<CustomerResponse>> GetOneCustomerAsync(int id)
//  {
//      return Ok(await customerService.GetCustomerAsync(id));
//  }
//
//  // Service: throws ServiceException if not found
//  public async Task<CustomerResponse> GetCustomerAsync(int customerId)
//  {
//      var customerResponse = await db.RegisteredCustomers
//          .Where(c => c.Id == customerId)
//          .Select(c => new CustomerResponse(c.Id, c.FirstName, c.LastName, c.Account.Email, c.Phone))
//          .FirstOrDefaultAsync()
//          ?? throw ServiceException.NotFound($"Customer not found: {customerId}");
//
//      return customerResponse;
//  }
//
// ──────────────────────────────────────────────────────────────────────
//  Example 2: Controller does business logic + error handling, returns success or error response directly
// ──────────────────────────────────────────────────────────────────────
//
//  [HttpGet("{id}")]
//  public async Task<ActionResult<CustomerResponse>> GetOneCustomerAsync(int id)
//  {
//      var customerResponse = await db.RegisteredCustomers
//          .Where(c => c.Id == id)
//          .Select(c => new CustomerResponse(c.Id, c.FirstName, c.LastName, c.Account.Email, c.Phone))
//          .FirstOrDefaultAsync();
//
//      if (customerResponse == null) return Problem(
//          title: "Not Found",
//          detail: $"Customer not found: {id}",
//          statusCode: StatusCodes.Status404NotFound);
//
//      return Ok(customerResponse);
//  }

// ══════════════════════════════════════════════════════════════════════

using Microsoft.AspNetCore.Mvc;
using SpengerbiteApi.Services;
using SpengerbiteApi.ViewModels;

namespace SpengerbiteApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(ICustomerService customerService ) : ControllerBase
{
    // GET /api/customers/{id} -> 200 OK
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerResponse>> GetOneCustomerAsync(int id)
    {
        return Ok(await customerService.GetCustomerAsync(id));
    }

    // POST /api/customers -> 201 Created + Location header
    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> CreateCustomerAsync(
        [FromBody] RegisterCustomerRequest request)
    {
        var customer = await customerService.RegisterCustomerAsync(request);
        return CreatedAtAction("GetOneCustomer", new { id = customer.Id }, customer);
    }

    // PUT /api/customers/{id} -> 200 OK (full update)
    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerResponse>> UpdateCustomerAsync(
        int id, [FromBody] UpdateCustomerRequest request)
    {
        return Ok(await customerService.UpdateCustomerAsync(id, request));
    }

    // PATCH /api/customers/{id} -> 200 OK (partial update, name only)
    [HttpPatch("{id}")]
    public async Task<ActionResult<CustomerResponse>> ChangeCustomerNameAsync(
        int id, [FromBody] ChangeCustomerNameRequest request)
    {
        return Ok(await customerService.ChangeCustomerNameAsync(id, request));
    }

    // DELETE /api/customers/{id} -> 204 No Content
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomerAsync(int id)
    {
        await customerService.DeleteCustomerAsync(id);
        return NoContent();
    }
}
