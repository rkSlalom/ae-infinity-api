using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Infrastructure.Persistence;
using AeInfinity.Infrastructure.Persistence.Repositories;
using AeInfinity.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AeInfinity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Use file-based SQLite for better concurrency support
        // This avoids the "unable to delete/modify user-function" error with shared in-memory connections
        var dbPath = Path.Combine(AppContext.BaseDirectory, "App_Data", "ae-infinity.db");
        var dbDirectory = Path.GetDirectoryName(dbPath);
        
        // Ensure the directory exists
        if (!Directory.Exists(dbDirectory))
        {
            Directory.CreateDirectory(dbDirectory!);
        }
        
        var connectionString = $"Data Source={dbPath};Cache=Shared;Pooling=True";

        // File-based SQLite Database with connection pooling
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString, sqliteOptions =>
            {
                sqliteOptions.CommandTimeout(30); // 30 seconds timeout
            })
            .EnableSensitiveDataLogging() // Helpful for debugging
            .EnableDetailedErrors());

        services.AddScoped<IApplicationDbContext>(provider => 
            provider.GetRequiredService<ApplicationDbContext>());

        // Register repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Register services
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasherService>();
        services.AddScoped<IListPermissionService, ListPermissionService>();

        return services;
    }
}

