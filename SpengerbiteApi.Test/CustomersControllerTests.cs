using System.Net;
using SpengerbiteApi.ViewModels;

namespace SpengerbiteApi.Test;

[Collection("Sequential")]
public class CustomersControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public CustomersControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetCustomer_ShouldReturn200_WhenCustomerExists()
    {
        // Given
        _factory.InitializeDatabase(db =>
        {
            db.RegisteredCustomers.Add(Fixtures.NewRegisteredCustomer());
            db.SaveChanges();
        });

        // When
        var (statusCode, customer) = await _factory.GetHttpContent<CustomerResponse>("/api/customers/1");

        // Then
        Assert.True(statusCode == HttpStatusCode.OK);
        Assert.NotNull(customer);
        Assert.Equal("Helene", customer.FirstName);
    }

    [Fact]
    public async Task GetCustomer_ShouldReturn404_WhenCustomerDoesNotExist()
    {
        // Given
        _factory.InitializeDatabase(db => { });

        // When
        var (statusCode, _) = await _factory.GetHttpContent<CustomerResponse>("/api/customers/999");

        // Then
        Assert.True(statusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturn201_WhenRequestIsValid()
    {
        // Given
        _factory.InitializeDatabase(db => { });

        var request = new
        {
            FirstName = "Max",
            LastName = "Mustermann",
            Email = "max@test.at",
            Password = "SecurePassword123!",
            Address = new { Street = "Teststr. 1", Zip = "1010", City = "Wien", Country = "AT" }
        };

        // When
        var (statusCode, body) = await _factory.PostHttpContent("/api/customers", request);

        // Then
        Assert.True(statusCode == HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturn409_WhenEmailAlreadyExists()
    {
        // Given
        _factory.InitializeDatabase(db =>
        {
            db.RegisteredCustomers.Add(Fixtures.NewRegisteredCustomer("Max", "Muster"));
            db.SaveChanges();
        });

        var request = new
        {
            FirstName = "Other",
            LastName = "Person",
            Email = "Max.Muster@spengergasse.at",
            Password = "SecurePassword123!",
            Address = new { Street = "Teststr. 1", Zip = "1010", City = "Wien", Country = "AT" }
        };

        // When
        var (statusCode, _) = await _factory.PostHttpContent("/api/customers", request);

        // Then
        Assert.True(statusCode == HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateCustomer_ShouldReturn200_WhenCustomerExists()
    {
        // Given
        _factory.InitializeDatabase(db =>
        {
            db.RegisteredCustomers.Add(Fixtures.NewRegisteredCustomer());
            db.SaveChanges();
        });

        var request = new
        {
            FirstName = "Updated",
            LastName = "Name",
            Phone = "9999999",
            Address = new { Street = "Neue Str. 5", Zip = "1020", City = "Wien", Country = "AT" }
        };

        // When
        var (statusCode, body) = await _factory.PutHttpContent("/api/customers/1", request);

        // Then
        Assert.True(statusCode == HttpStatusCode.OK);
        Assert.Equal("Updated", body.GetProperty("firstName").GetString());
    }

    [Fact]
    public async Task UpdateCustomer_ShouldReturn404_WhenCustomerDoesNotExist()
    {
        // Given
        _factory.InitializeDatabase(db => { });

        var request = new
        {
            FirstName = "Updated",
            LastName = "Name",
            Phone = "9999999",
            Address = new { Street = "Str. 1", Zip = "1010", City = "Wien", Country = "AT" }
        };

        // When
        var (statusCode, _) = await _factory.PutHttpContent("/api/customers/999", request);

        // Then
        Assert.True(statusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ChangeCustomerName_ShouldReturn200_WhenCustomerExists()
    {
        // Given
        _factory.InitializeDatabase(db =>
        {
            db.RegisteredCustomers.Add(Fixtures.NewRegisteredCustomer());
            db.SaveChanges();
        });

        var request = new { FirstName = "NewFirst", LastName = "NewLast" };

        // When
        var (statusCode, body) = await _factory.PatchHttpContent("/api/customers/1", request);

        // Then
        Assert.True(statusCode == HttpStatusCode.OK);
        Assert.Equal("NewFirst", body.GetProperty("firstName").GetString());
    }

    [Fact]
    public async Task DeleteCustomer_ShouldReturn204_WhenCustomerExists()
    {
        // Given
        _factory.InitializeDatabase(db =>
        {
            db.RegisteredCustomers.Add(Fixtures.NewRegisteredCustomer());
            db.SaveChanges();
        });

        // When
        var statusCode = await _factory.DeleteHttpContent("/api/customers/1");

        // Then
        Assert.True(statusCode == HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteCustomer_ShouldReturn404_WhenCustomerDoesNotExist()
    {
        // Given
        _factory.InitializeDatabase(db => { });

        // When
        var statusCode = await _factory.DeleteHttpContent("/api/customers/999");

        // Then
        Assert.True(statusCode == HttpStatusCode.NotFound);
    }
}
