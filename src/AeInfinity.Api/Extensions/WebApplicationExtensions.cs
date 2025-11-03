using AeInfinity.Infrastructure.Persistence;

namespace AeInfinity.Api.Extensions;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> InitializeDatabaseAsync(this WebApplication app)
    {
        await DbInitializer.InitializeAsync(app.Services);
        return app;
    }
}

