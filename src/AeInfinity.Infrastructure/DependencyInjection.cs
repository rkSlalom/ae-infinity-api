using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Infrastructure.Persistence;
using AeInfinity.Infrastructure.Persistence.Repositories;
using AeInfinity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AeInfinity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database configuration (SQL Server for Azure)
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

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

