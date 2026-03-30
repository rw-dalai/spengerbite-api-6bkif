using System.Diagnostics;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpengerbiteApi.Infrastructure;

namespace SpengerbiteApi.Test;

// SUTXXX_ShouldXXX_WhenXXX
// SUT = System Under Test
// Should = expected behavior
// When = condition under which the expected behavior should occur

// RegisteredCustomer_ShouldPersist
public class SpengerbiteContextTests
{
    private SpengerbiteContext GetDatabase()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder()
            .UseSqlite(connection)
            .LogTo(message => Debug.WriteLine(message), LogLevel.Information)
            .EnableSensitiveDataLogging()
            .Options;

        var db = new SpengerbiteContext(options);
        Debug.WriteLine(db.Database.GenerateCreateScript());

        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        return db;
    }

    [Fact]
    public void CreateDatabaseSuccessTest()
    {
        using var db = GetDatabase();
        Assert.True(db.Database.CanConnect());
    }

    [Fact]
    public void RegisteredCustomer_ShouldSaveAndRetrieve()
    {
        using var db = GetDatabase();
        
        // Given
        var customer = Fixtures.NewRegisteredCustomer(
            firstName: "Ana", lastName: "Musterfrau");
        
        // When
        db.Customers.Add(customer);
        db.SaveChanges();
        db.ChangeTracker.Clear();
        
        // Then
        // var retrievedCustomer = db.Customers.Find(customer.Id)
        // var retrievedCustomer = db.Customers.FirstOrDefault(c => c.Id == customer.Id);
        
        var retrievedCustomer = db.RegisteredCustomers
            // Include: loads Account, otherwise null (INNER JOIN)
            .Include(rc => rc.Account)
            .FirstOrDefault(c => c.Id == customer.Id);
        // var retrievedCustomer = db.RegisteredCustomers.Find(customer.Id);
        
        Assert.NotNull(retrievedCustomer);
        Assert.NotNull(retrievedCustomer.Account);
    }
}
