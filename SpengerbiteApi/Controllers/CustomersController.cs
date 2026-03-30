using Microsoft.AspNetCore.Mvc;
using SpengerbiteApi.Models.Customer;
using SpengerbiteApi.Services;
using SpengerbiteApi.ViewModels;

namespace SpengerbiteApi.Controllers;


// https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html

// Software Design Principle
// Depend on abstractions, not on concretions.

// SOLID Principles
// Single Responsibility Principle
// Open/Closed Principle
// Liskov Substitution Principle
// Interface Segregation Principle
// Dependency Inversion Principle

[ApiController]
[Route("api/[controller]")]
public class CustomersController(
    ILogger<CustomersController> logger,
    ICustomerService customerService
    
) : ControllerBase
{
    // api/customers
    
    // HTTP 200 OK
    // HTTP Body: existing resource (RegisteredCustomer)
    [HttpGet("{id}")]
    public async Task<ActionResult<RegisteredCustomer>> GetOneCustomer(
        [FromRoute] int id
    )
    {
        RegisteredCustomer? customer = await customerService.GetCustomerAsync(id);
        
        if (customer is null) return NotFound();
        
        return Ok(customer);
    }

    // HTTP 201 Created
    // HTTP Location header: URL of the newly created resource: /api/customers/1
    // HTTP Body: newly created resource (RegisteredCustomer)
    [HttpPost]
    public async Task<ActionResult<RegisteredCustomer>> CreateCustomer(
        [FromBody] RegisterCustomerRequest request)
    {
        // try
        // {
            RegisteredCustomer customer = await customerService.RegisterCustomerAsync(request);
            
            return CreatedAtAction(nameof(GetOneCustomer), new { id = customer.Id }, customer);

        // }
        // We use GlobalExceptionHandler to handle exceptions globally, so we don't need to catch exceptions here
        // catch (Exception ex)
        // {
            // return Problem(
                // detail: ex.Message,
                // statusCode: StatusCodes.Status400BadRequest
            // );
        // }
        
    }
}