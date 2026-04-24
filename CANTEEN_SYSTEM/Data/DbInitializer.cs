using CANTEEN_SYSTEM.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CANTEEN_SYSTEM.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<CanteenDbContext>();

        // Works for both providers here:
        // - SQLite creates the local file on first run
        // - Azure SQL creates the schema in the selected database
        await db.Database.EnsureCreatedAsync();

        if (!await db.Products.AnyAsync())
        {
            // Seed demo menu items once so the app is usable on a fresh database.
            db.Products.AddRange(
                new Product { Name = "Chicken Adobo with Rice", Category = "Meals", Price = 75m, Stock = 20, ImageUrl = "https://images.unsplash.com/photo-1626082927389-6cd097cdc6ec?w=400" },
                new Product { Name = "Pork Sinigang with Rice", Category = "Meals", Price = 80m, Stock = 15, ImageUrl = "https://images.unsplash.com/photo-1455619452474-d2be8b1e70cd?w=400" },
                new Product { Name = "Fried Chicken with Rice", Category = "Meals", Price = 70m, Stock = 25, ImageUrl = "https://images.unsplash.com/photo-1626645738196-c2a7c87a8f58?w=400" },
                new Product { Name = "Pancit Canton", Category = "Meals", Price = 50m, Stock = 30, ImageUrl = "https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43?w=400" },
                new Product { Name = "Beef Tapa with Rice", Category = "Meals", Price = 85m, Stock = 12, ImageUrl = "https://images.unsplash.com/photo-1529692236671-f1f6cf9683ba?w=400" },
                new Product { Name = "Lumpia Shanghai (5pcs)", Category = "Meals", Price = 40m, Stock = 50, ImageUrl = "https://images.unsplash.com/photo-1534422298391-e4f8c172dddb?w=400" },
                new Product { Name = "Iced Tea", Category = "Drinks", Price = 20m, Stock = 50, ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?w=400" },
                new Product { Name = "Bottled Water", Category = "Drinks", Price = 15m, Stock = 100, ImageUrl = "https://images.unsplash.com/photo-1548839140-29a749e1cf4d?w=400" },
                new Product { Name = "Fresh Buko Juice", Category = "Drinks", Price = 35m, Stock = 20, ImageUrl = "https://images.unsplash.com/photo-1546173159-315724a31696?w=400" },
                new Product { Name = "Mango Shake", Category = "Drinks", Price = 45m, Stock = 15, ImageUrl = "https://images.unsplash.com/photo-1623065422902-30a2d299bbe4?w=400" },
                new Product { Name = "Soft Drinks", Category = "Drinks", Price = 25m, Stock = 60, ImageUrl = "https://images.unsplash.com/photo-1622483767028-3f66f32aef97?w=400" },
                new Product { Name = "Banana Cue", Category = "Snacks", Price = 15m, Stock = 40, ImageUrl = "https://images.unsplash.com/photo-1603833665858-e61d17a86224?w=400" },
                new Product { Name = "Turon", Category = "Snacks", Price = 20m, Stock = 35, ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=400" },
                new Product { Name = "Ensaymada", Category = "Snacks", Price = 25m, Stock = 30, ImageUrl = "https://images.unsplash.com/photo-1586985289688-ca3cf47d3e6e?w=400" },
                new Product { Name = "Cheese Sticks", Category = "Snacks", Price = 30m, Stock = 25, ImageUrl = "https://images.unsplash.com/photo-1599490659213-e2b9527bd087?w=400" },
                new Product { Name = "French Fries", Category = "Snacks", Price = 35m, Stock = 20, ImageUrl = "https://images.unsplash.com/photo-1573080496219-bb080dd4f877?w=400" }
            );
        }

        if (!await db.Employees.AnyAsync())
        {
            // Seed a default admin account for first-time staff access.
            db.Employees.Add(
                new Employee
                {
                    Name = "Admin",
                    QrCode = "ADMIN001",
                    Pin = "1234",
                    Role = "admin",
                    CreatedAt = DateTime.UtcNow
                });
        }

        await db.SaveChangesAsync();
    }
}
