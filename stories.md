# SpengeBite Stories

Work through these stories in order. Each story has TODO comments in the listed files.
Test your work via Scalar UI at `/scalar/v1` after each story.

---

### Story 1: Customer Registration (already implemented)
> As a customer, I want to register with my name, email, address and password.

**Files:**
- [x] `Controllers/CustomersController.cs`
- [x] `Services/CustomerService.cs`
- [x] `Services/ICustomerService.cs`
- [x] `ViewModels/RegisterCustomerRequest.cs`
- [x] `ViewModels/CustomerResponse.cs`
- [x] `Exceptions/ServiceException.cs`
- [x] `Exceptions/GlobalExceptionHandler.cs`

---

### Story 2: Password Hashing
> As a customer, I want my password stored securely so it cannot be read from the database.

**Files:**
- [ ] `Services/PasswordService.cs` -- hash + verify password
- [ ] `Program.cs` -- register PasswordService + set 600k iterations

**Verify:** POST /api/customers with a new customer.
The password in the database should be a hash, not plaintext. Try a weak password like "123" and it should be rejected.

---

### Story 3: View Orders (already implemented)
> As a customer, I want to see my orders sorted by newest first, excluding cancelled ones.

**Files:**
- [x] `Controllers/OrderController.cs`
- [x] `Services/OrderService.cs`
- [x] `Services/IOrderService.cs`
- [x] `ViewModels/OrderSummaryResponse.cs`

---

### Story 4: Add Item to Cart
> As a customer, I want to add an item to my cart.

- `CartResponse CartService.AddItemToCart(CustomerId, AddCartItemRequest)`  
- `AddCartItemRequest` should include `MenuItemId` and `Quantity`.  
- `CartResponse` should include `CartId`, `CustomerId`, `RestaurantId`, `TotalPrice`, and a List of `CartItemResponse`.  
- `CartItemResponse` should include `MenuItemId`, `MenuItemName`, `Price`, `Quantity`, and `LineTotal`.  


**Files:**
- [ ] `ViewModels/AddCartItemRequest.cs` -- **create** request record
- [ ] `ViewModels/CartResponse.cs` -- **create** response record
- [ ] `ViewModels/CartItemResponse.cs` -- **create** response record
- [ ] `Models/Cart/Cart.cs` -- implement AddOrIncreaseItem()
- [ ] `Services/ICartService.cs` -- add method signature
- [ ] `Services/CartService.cs` -- load entities, validate, add item, save
- [ ] `Controllers/CartController.cs` -- add POST endpoint
- [ ] `Program.cs` -- register CartService as Scoped

**Verify:** POST /api/customers/1/cart/items with `{"menuItemId": 1, "quantity": 2}` -> 200.

---

### Story 5: Clear Cart
> As a customer, I want to clear my cart.

`void CartService.ClearCart(CustomerId)`

**Files:**
- [ ] `Models/Cart/Cart.cs` -- implement ClearItems()
- [ ] `Services/ICartService.cs` -- add method signature
- [ ] `Services/CartService.cs` -- load cart, clear items, save
- [ ] `Controllers/CartController.cs` -- add DELETE endpoint

**Verify:** DELETE /api/customers/1/cart -> 204.

---

### Story 6: Cancel Order
> As a customer, I want to cancel my order.

`void OrderService.CancelOrder(CustomerId, OrderId)`

**Files:**
- [ ] `Models/Order/Order.cs` -- implement Cancel()
- [ ] `Services/IOrderService.cs` -- add method signature
- [ ] `Services/OrderService.cs` -- load order, verify ownership, cancel
- [ ] `Controllers/OrderController.cs` -- add POST cancel endpoint
- [ ] `Program.cs` -- register OrderService as Scoped

**Verify:** POST /api/customers/1/orders/{orderId}/cancel -> 204. Cancelling a delivered order -> 400.

---

### Story 7: Place Order (depends on Story 5)
> As a customer, I want to place an order from my cart.

`OrderResponse OrderService.Order(CustomerId)`
`OrderResponse` should include `OrderId`, `UserId`, `RestaurantId`, `Status`, `TotalPrice`, and a List of `OrderItemResponse`
`OrderItemResponse` should include `MenuItemName`, `Quantity`, `Price`, and `LineTotal`.

**Files:**
- [ ] `ViewModels/OrderResponse.cs` -- **create** response record
- [ ] `ViewModels/OrderItemResponse.cs` -- **create** response record
- [ ] `Models/Cart/Cart.cs` -- implement IsEmpty
- [ ] `Models/Order/Order.cs` -- implement AddLinesFromCart()
- [ ] `Services/IOrderService.cs` -- add method signature
- [ ] `Services/OrderService.cs` -- load cart, create order, clear cart, save
- [ ] `Controllers/OrderController.cs` -- add POST endpoint

**Verify:** POST /api/customers/1/orders -> 201 + Location header. Cart should be empty after.
