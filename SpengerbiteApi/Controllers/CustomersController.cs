using Microsoft.AspNetCore.Mvc;

namespace SpengerbiteApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(ILogger<CustomersController> logger ) : ControllerBase
{
}