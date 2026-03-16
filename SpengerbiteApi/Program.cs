using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SpengerbiteApi.Infrastructure;


// --- 1. Builder Phase ---
// WHY: Register Services and configure the WebApplicationBuilder (DI, logging, config, Kestrel, etc.)


// Creates a WebApplicationBuilder with default config (appsettings.json, env variables, logging, Kestrel)
var builder = WebApplication.CreateBuilder(args);

// Registers controller services (incl. model binding, routing)
builder.Services.AddControllers();


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
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SpengerbiteContext>();
    db.Database.EnsureCreated();
}


// --- 3. Middleware Phase ---
// WHY: Configure the Order of Middleware (e.g. routing, auth, error handling) and map endpoints before starting the server.

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
