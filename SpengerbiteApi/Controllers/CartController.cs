using Microsoft.AspNetCore.Mvc;
using SpengerbiteApi.Services;

namespace SpengerbiteApi.Controllers;

[ApiController]
[Route("api/customers/{customerId}/cart")]
public class CartController(
    ICartService cartService
) : ControllerBase
{

    // ═══════════════════════════════════════════════════════════
    // Story 4: Add item to cart
    // ═══════════════════════════════════════════════════════════
    // TODO: Add [HttpPost("items")] endpoint for adding an item to the cart
    //   Parameters: int customerId (from route), AddCartItemRequest (from body)
    //   Return: Ok(response) with the updated cart -> 200 OK

    // ═══════════════════════════════════════════════════════════
    // Story 5: Clear cart
    // ═══════════════════════════════════════════════════════════
    // TODO: Add [HttpDelete] endpoint for clearing the cart
    //   Parameter: int customerId (from route)
    //   Return: NoContent() -> 204 No Content
}
