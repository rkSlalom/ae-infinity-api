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
        // Create a persistent SQLite in-memory connection
        var keepAliveConnection = new SqliteConnection("DataSource=InMemoryDb;Mode=Memory;Cache=Shared");
        keepAliveConnection.Open();
        
        // Register the connection as singleton to keep it alive
        services.AddSingleton(keepAliveConnection);

        // SQLite In-Memory Database with shared cache
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(keepAliveConnection));

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

