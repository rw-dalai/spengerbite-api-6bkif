using Microsoft.EntityFrameworkCore;
using SpengerbiteApi.Models.Auth;
using SpengerbiteApi.Models.Cart;
using SpengerbiteApi.Models.Customer;
using SpengerbiteApi.Models.Order;
using SpengerbiteApi.Models.Restaurant;
using SpengerbiteApi.Models.RestaurantOwner;
using SpengerbiteApi.Models.Shared;

namespace SpengerbiteApi.Infrastructure;

public class SpengerbiteContext(DbContextOptions option) : DbContext(option)
{
    // --- Users ---
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<RestaurantOwner> RestaurantOwners => Set<RestaurantOwner>();

    // --- Restaurant ---
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();

    // --- Cart ---
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    // --- Order ---
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Relationships
        modelBuilder.Entity<RegisteredCustomer>()
            .HasOne(rc => rc.Account)
            .WithOne()
            .HasForeignKey<RegisteredCustomer>("AccountId")
            .IsRequired(); // Because of TPH, the DB column may still be nullable.

        modelBuilder.Entity<RestaurantOwner>()
            .HasOne(ro => ro.Account)
            .WithOne()
            .HasForeignKey<RestaurantOwner>("AccountId")
            .IsRequired();

        // Rich Types
        modelBuilder.Entity<UserAccount>().Property(ua => ua.Email).HasConversion(
            objVal => objVal.Value, dbVal => new Email(dbVal));

        modelBuilder.Entity<UserAccount>().Property(ua => ua.PasswordHash).HasConversion(
            objVal => objVal.Value, dbVal => new PasswordHash(dbVal));

        modelBuilder.Entity<RegisteredCustomer>().Property(rc => rc.Phone).HasConversion(
            objVal => objVal == null ? null : objVal.Value,
            dbVal => dbVal == null ? null : new Phone(dbVal));

        modelBuilder.Entity<RestaurantOwner>().Property(ro => ro.Phone).HasConversion(
            objVal => objVal.Value, dbVal => new Phone(dbVal));

        modelBuilder.Entity<Restaurant>().Property(r => r.Phone).HasConversion(
            objVal => objVal.Value, dbVal => new Phone(dbVal));

        // Value Objects
        modelBuilder.Entity<RegisteredCustomer>().OwnsOne(rc => rc.Address);
        modelBuilder.Entity<RestaurantOwner>().OwnsOne(ro => ro.CompanyAddress);
        modelBuilder.Entity<Restaurant>().OwnsOne(r => r.Address);
        modelBuilder.Entity<Restaurant>().OwnsOne(r => r.OpeningHours);

        // Enums
        modelBuilder.Entity<UserAccount>().Property(ua => ua.Role).HasConversion<string>();
        modelBuilder.Entity<Order>().Property(o => o.Status).HasConversion<string>();

        // Decimal Precision
        modelBuilder.Entity<MenuItem>().Property(mi => mi.Price).HasPrecision(9, 2);
        modelBuilder.Entity<OrderItem>().Property(oi => oi.Price).HasPrecision(9, 2);

        // Indexes
        modelBuilder.Entity<UserAccount>().HasIndex("Email").IsUnique();

        // Inheritance
        modelBuilder.Entity<Customer>().HasDiscriminator<string>("CustomerType")
            .HasValue<RegisteredCustomer>("Registered")
            .HasValue<AnonymousCustomer>("Anonymous");
    }
}
