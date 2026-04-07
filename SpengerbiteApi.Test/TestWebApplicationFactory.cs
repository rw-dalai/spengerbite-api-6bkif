using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SpengerbiteApi.Infrastructure;
using SpengerbiteApi.Models.Auth;
using SpengerbiteApi.Models.Shared;
using SpengerbiteApi.ViewModels.Converters;
using System.Net;
using System.Text;
using System.Text.Json;

namespace SpengerbiteApi.Test;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new EmailConverter(), new PhoneConverter() }
    };

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.First(d => d.ServiceType == typeof(DbContextOptions<SpengerbiteContext>));
            services.Remove(descriptor);
            services.AddDbContext<SpengerbiteContext>(options =>
            {
                options.UseSqlite("DataSource=spengerbite_test.db");
            });

            // Register a test PasswordService that skips zxcvbn and just hashes
            services.AddSingleton<SpengerbiteApi.Services.IPasswordService, TestPasswordService>();
        });
        builder.UseEnvironment("Testing");
    }

    public void InitializeDatabase(Action<SpengerbiteContext> action)
    {
        using var scope = Services.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<SpengerbiteContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        action(db);
    }

    public Tout QueryDatabase<Tout>(Func<SpengerbiteContext, Tout> query)
    {
        using var scope = Services.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<SpengerbiteContext>();
        return query(db);
    }

    // GET with strongly typed response
    public async Task<(HttpStatusCode, T?)> GetHttpContent<T>(string requestUrl) where T : class
    {
        using var client = CreateClient();
        var response = await client.GetAsync(requestUrl);
        if (!response.IsSuccessStatusCode) return (response.StatusCode, default);
        var dataString = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<T>(dataString, _jsonOptions);
        return (response.StatusCode, data);
    }

    // POST with JSON body
    public async Task<(HttpStatusCode, JsonElement)> PostHttpContent<T>(string requestUrl, T payload) where T : class
    {
        using var client = CreateClient();
        var jsonBody = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(requestUrl, jsonBody);
        if (!response.IsSuccessStatusCode) return (response.StatusCode, new JsonElement());
        var dataString = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(dataString)) return (response.StatusCode, new JsonElement());
        var data = JsonDocument.Parse(dataString);
        return (response.StatusCode, data.RootElement);
    }

    // PUT with JSON body
    public async Task<(HttpStatusCode, JsonElement)> PutHttpContent<T>(string requestUrl, T payload) where T : class
    {
        using var client = CreateClient();
        var jsonBody = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await client.PutAsync(requestUrl, jsonBody);
        if (!response.IsSuccessStatusCode) return (response.StatusCode, new JsonElement());
        var dataString = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(dataString)) return (response.StatusCode, new JsonElement());
        var data = JsonDocument.Parse(dataString);
        return (response.StatusCode, data.RootElement);
    }

    // PATCH with JSON body
    public async Task<(HttpStatusCode, JsonElement)> PatchHttpContent<T>(string requestUrl, T payload) where T : class
    {
        using var client = CreateClient();
        var jsonBody = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await client.PatchAsync(requestUrl, jsonBody);
        if (!response.IsSuccessStatusCode) return (response.StatusCode, new JsonElement());
        var dataString = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(dataString)) return (response.StatusCode, new JsonElement());
        var data = JsonDocument.Parse(dataString);
        return (response.StatusCode, data.RootElement);
    }

    // DELETE
    public async Task<HttpStatusCode> DeleteHttpContent(string requestUrl)
    {
        using var client = CreateClient();
        var response = await client.DeleteAsync(requestUrl);
        return response.StatusCode;
    }
}

// Simple test implementation that skips zxcvbn strength check
public class TestPasswordService : SpengerbiteApi.Services.IPasswordService
{
    private readonly PasswordHasher<UserAccount> _hasher = new();

    public PasswordHash HashPassword(string plaintext)
        => new PasswordHash(_hasher.HashPassword(null!, plaintext));

    public bool VerifyPassword(UserAccount account, string plaintext)
        => _hasher.VerifyHashedPassword(null!, account.PasswordHash.Value, plaintext)
           != PasswordVerificationResult.Failed;
}
