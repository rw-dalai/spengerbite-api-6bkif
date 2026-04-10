using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SpengerbiteApi.Exceptions;
using SpengerbiteApi.Infrastructure;
using SpengerbiteApi.Models;
using SpengerbiteApi.Services;
using SpengerbiteApi.ViewModels.Converters;


// --- 1. Builder Phase ---
// WHY: Register Services and configure the WebApplicationBuilder (DI, logging, config, Kestrel, etc.)


// Creates a WebApplicationBuilder with default config (appsettings.json, env variables, logging, Kestrel)
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// builder.Services.AddTransient()
builder.Services.AddScoped<ICustomerService, CustomerService>();

// Story 2: Register PasswordService
// TODO: Register IPasswordService -> PasswordService as Singleton (stateless, no DbContext)
// TODO: Increase PBKDF2 iterations to 600,000 (OWASP minimum)
//   Use: builder.Services.Configure<PasswordHasherOptions>(options => options.IterationCount = 600_000);

// Story 4: Register CartService
// TODO: Register ICartService -> CartService as Scoped (depends on DbContext)

// Story 6: Register OrderService
// TODO: Register IOrderService -> OrderService as Scoped (depends on DbContext)
builder.Services.AddSingleton<PasswordHasher<UserAccount>>();

// Registers controller services (incl. model binding, routing)
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new EmailConverter());
    options.JsonSerializerOptions.Converters.Add(new PhoneConverter());
});

// Registers SpengerbiteContext
builder.Services.AddDbContext<SpengerbiteContext>(options =>
    options.UseSqlite("DataSource=spengerbite.db"));

// Registers OpenAPI
builder.Services.AddOpenApi();

// Builds the WebApplication
var app = builder.Build();


// --- 2. Initialization Phase ---
// WHY: Perform app initialization tasks (e.g. database setup) before handling requests.

// Creates the database if it doesn't exist
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<SpengerbiteContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<PasswordHasher<UserAccount>>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
    DatabaseSeeder.Seed(db, passwordHasher);
}


// --- 3. Middleware Phase ---
// WHY: Configure the Order of Middleware (e.g. routing, auth, error handling) and map endpoints before starting the server.

app.UseExceptionHandler();

// TODO: Enable later when auth is implemented
// app.UseAuthentication();
// app.UseAuthorization();

// Exposes the OpenAPI spec at /openapi/v1.json
app.MapOpenApi();

// Serves the Scalar UI at /scalar/v1 (interactive API docs)
app.MapScalarApiReference();

// Maps controller routes so incoming HTTP requests
app.MapControllers();

// Starts Kestrel
app.Run();
