using System.Text;
using AeInfinity.Api.Extensions;
using AeInfinity.Api.Hubs;
using AeInfinity.Api.Middleware;
using AeInfinity.Api.Services.Realtime;
using AeInfinity.Application;
using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Use camelCase for JSON serialization (JavaScript/TypeScript standard)
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "AE Infinity API", 
        Version = "v1",
        Description = "A Clean Architecture ASP.NET Core Web API"
    });
});

// Add Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSecret = builder.Configuration["Jwt:Secret"] 
        ?? throw new InvalidOperationException("JWT Secret is not configured");
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ClockSkew = TimeSpan.Zero
    };

    // Configure JWT authentication for SignalR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Allow SignalR to read token from query string
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Add SignalR
builder.Services.AddSignalR(options =>
{
    // Configure SignalR options
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
})
.AddJsonProtocol(options =>
{
    // Use camelCase for SignalR JSON serialization
    options.PayloadSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

// Register realtime services
builder.Services.AddSingleton<IConnectionManager, ConnectionManager>();
builder.Services.AddScoped<IRealtimeNotificationService, RealtimeNotificationService>();

// Add CORS with WebSocket support
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // Frontend URLs
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Required for SignalR WebSocket
    });
});

var app = builder.Build();

// Initialize database
await app.InitializeDatabaseAsync();

// Configure the HTTP request pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

// CORS must be before authentication and authorization
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AE Infinity API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

// Disable HTTPS redirection in development for easier testing
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

// Map SignalR hubs with CORS
app.MapHub<ShoppingListHub>("/hubs/shopping-list")
   .RequireCors("AllowAll");

app.MapControllers();

Log.Information("Starting AE Infinity API with SignalR support...");

app.Run();

