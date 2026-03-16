using System.Diagnostics;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SpengerbiteApi.Infrastructure;

namespace SpengerbiteApi.Test;

public class SpengerbiteContextTests
{
    private SpengerbiteContext GetDatabase()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder()
            .UseSqlite(connection)
            .LogTo(message => Debug.WriteLine(message), Microsoft.Extensions.Logging.LogLevel.Information)
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
}
