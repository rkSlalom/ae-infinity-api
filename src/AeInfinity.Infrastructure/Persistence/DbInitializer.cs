using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AeInfinity.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
        
        try
        {
            // Ensure database is created (for in-memory database)
            await context.Database.EnsureCreatedAsync();
            logger.LogInformation("Database created successfully");

            // Seed data
            var seeder = new DbSeeder(context);
            await seeder.SeedAllAsync();
            logger.LogInformation("Database seeded successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }
}

