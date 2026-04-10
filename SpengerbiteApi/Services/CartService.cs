using Microsoft.EntityFrameworkCore;
using SpengerbiteApi.Infrastructure;
using SpengerbiteApi.Exceptions;

namespace SpengerbiteApi.Services;

public class CartService(SpengerbiteContext db, ILogger<CartService> logger ) : ICartService
{
    // ═══════════════════════════════════════════════════════════
    // Story 4: Add item to cart
    // ═══════════════════════════════════════════════════════════
    // TODO Step 1: Load the customer (throw not found if missing)
    // TODO Step 2: Load the menu item with its restaurant (throw not found if missing)
    // TODO Step 3: Load existing cart or create a new one
    // TODO Step 4: Validate same restaurant (throw conflict if cart is for a different restaurant)
    // TODO Step 5: Add or increase the item
    // TODO Step 6: Save and return mapped response

    // ═══════════════════════════════════════════════════════════
    // Story 5: Clear cart
    // ═══════════════════════════════════════════════════════════
    // TODO Step 1: Load the customer (throw not found if missing)
    // TODO Step 2: Load the cart with its items (throw not found if missing)
    // TODO Step 3: Clear all items, save and log
}
