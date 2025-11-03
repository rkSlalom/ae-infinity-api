# AE Infinity API

A modern .NET 9.0 Web API built with **Clean Architecture** principles, CQRS pattern, and Entity Framework Core with SQLite in-memory database.

## 🏛️ Architecture Overview

This project follows **Clean Architecture** with clear separation of concerns across four main layers:

```
┌─────────────────────────────────────────────┐
│           Presentation Layer (API)          │
│         Controllers, Middleware             │
└────────────────┬────────────────────────────┘
                 │
┌────────────────▼────────────────────────────┐
│        Infrastructure Layer                 │
│    Database, Repositories, Services         │
└────────────────┬────────────────────────────┘
                 │
┌────────────────▼────────────────────────────┐
│         Application Layer                   │
│    Use Cases (CQRS), DTOs, Interfaces       │
└────────────────┬────────────────────────────┘
                 │
┌────────────────▼────────────────────────────┐
│           Domain Layer                      │
│      Entities, Value Objects, Events        │
└─────────────────────────────────────────────┘
```

### Layer Dependencies

- **Domain**: No dependencies (Core business logic)
- **Application**: Depends on Domain
- **Infrastructure**: Depends on Application & Domain
- **API**: Depends on Infrastructure & Application

## 🚀 Features

### Architecture & Design Patterns
- ✅ **Clean Architecture** - Separation of concerns with clear boundaries
- ✅ **CQRS Pattern** - Using MediatR for command/query separation
- ✅ **Repository Pattern** - Data access abstraction
- ✅ **Dependency Injection** - Built-in ASP.NET Core DI container
- ✅ **Result Pattern** - Functional error handling

### Technologies & Frameworks
- ✅ **.NET 9.0** - Latest .NET framework
- ✅ **Entity Framework Core 9.0** - ORM with Code First approach
- ✅ **SQLite In-Memory Database** - Fast, lightweight data storage
- ✅ **MediatR** - CQRS and mediator pattern implementation
- ✅ **FluentValidation** - Input validation with fluent API
- ✅ **AutoMapper** - Object-to-object mapping
- ✅ **Swagger/OpenAPI** - Interactive API documentation
- ✅ **Serilog** - Structured logging

### API Features
- ✅ RESTful API endpoints
- ✅ Global exception handling middleware
- ✅ Validation with detailed error responses
- ✅ CORS support
- ✅ Swagger UI at root path

## 📋 Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Git](https://git-scm.com/)
- A code editor (Visual Studio, VS Code, or Rider)

## 🛠️ Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/rkSlalom/ae-infinity-api.git
cd ae-infinity-api
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Solution

```bash
dotnet build
```

### 4. Run the Application

```bash
cd src/AeInfinity.Api
dotnet run
```

The API will be available at:
- **HTTP**: http://localhost:5233
- **HTTPS**: https://localhost:7184
- **Swagger UI**: http://localhost:5233/ (root path)

## 📁 Project Structure

```
ae-infinity-api/
├── src/
│   ├── AeInfinity.Domain/                    # Core Domain Layer
│   │   ├── Common/                           # Base entities and interfaces
│   │   ├── Entities/                         # Domain entities
│   │   ├── Exceptions/                       # Domain exceptions
│   │   └── Events/                           # Domain events
│   │
│   ├── AeInfinity.Application/               # Application Layer
│   │   ├── Common/
│   │   │   ├── Behaviors/                    # MediatR pipeline behaviors
│   │   │   ├── Interfaces/                   # Application interfaces
│   │   │   ├── Mappings/                     # AutoMapper profiles
│   │   │   └── Models/                       # DTOs and Result types
│   │   ├── Features/                         # Feature-based organization
│   │   │   └── Products/                     # Product feature
│   │   │       ├── Commands/                 # CQRS Commands
│   │   │       │   ├── CreateProduct/
│   │   │       │   ├── UpdateProduct/
│   │   │       │   └── DeleteProduct/
│   │   │       └── Queries/                  # CQRS Queries
│   │   │           ├── GetProducts/
│   │   │           └── GetProductById/
│   │   └── DependencyInjection.cs            # Service registration
│   │
│   ├── AeInfinity.Infrastructure/            # Infrastructure Layer
│   │   ├── Persistence/
│   │   │   ├── Configurations/               # EF Core entity configurations
│   │   │   ├── Repositories/                 # Repository implementations
│   │   │   ├── ApplicationDbContext.cs       # DbContext
│   │   │   └── DbInitializer.cs              # Database initialization
│   │   └── DependencyInjection.cs            # Service registration
│   │
│   └── AeInfinity.Api/                       # Presentation Layer
│       ├── Controllers/                      # API Controllers
│       │   ├── BaseApiController.cs
│       │   └── ProductsController.cs
│       ├── Middleware/                       # Custom middleware
│       │   └── ExceptionHandlingMiddleware.cs
│       ├── Extensions/                       # Extension methods
│       ├── Properties/
│       │   └── launchSettings.json
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── AeInfinity.Api.http              # HTTP request samples
│       └── Program.cs                        # Application entry point
│
├── AeInfinity.sln                            # Solution file
├── .editorconfig                             # Code style configuration
├── .gitignore                                # Git ignore rules
└── README.md                                 # This file
```

## 📚 API Documentation

### Swagger UI

Once running, access the interactive API documentation at:
- **Swagger UI**: http://localhost:5233/

### Products API Endpoints

#### Get All Products
```http
GET /api/products
```

**Response:**
```json
[
  {
    "id": "11111111-1111-1111-1111-111111111111",
    "name": "Laptop",
    "description": "High-performance laptop for developers",
    "price": 1299.99,
    "stock": 50,
    "isActive": true,
    "createdAt": "2025-11-03T12:00:00Z"
  }
]
```

#### Get Product by ID
```http
GET /api/products/{id}
```

#### Create Product
```http
POST /api/products
Content-Type: application/json

{
  "name": "Wireless Mouse",
  "description": "Ergonomic wireless mouse",
  "price": 29.99,
  "stock": 100
}
```

**Response:** `201 Created` with product ID

#### Update Product
```http
PUT /api/products/{id}
Content-Type: application/json

{
  "id": "11111111-1111-1111-1111-111111111111",
  "name": "Updated Product",
  "description": "Updated description",
  "price": 39.99,
  "stock": 75,
  "isActive": true
}
```

**Response:** `204 No Content`

#### Delete Product
```http
DELETE /api/products/{id}
```

**Response:** `204 No Content`

### Validation

All requests are validated using FluentValidation. Invalid requests return `400 Bad Request` with detailed error information:

```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "errors": [
    {
      "property": "Name",
      "message": "Product name is required."
    },
    {
      "property": "Price",
      "message": "Product price must be greater than zero."
    }
  ]
}
```

## 🏗️ Development Guidelines

### Adding a New Feature

1. **Domain Layer**: Create entity in `Domain/Entities/`
2. **Application Layer**: 
   - Create commands in `Application/Features/{Feature}/Commands/`
   - Create queries in `Application/Features/{Feature}/Queries/`
   - Add validators for commands
   - Update `MappingProfile.cs`
3. **Infrastructure Layer**:
   - Add entity configuration in `Persistence/Configurations/`
   - Update `IApplicationDbContext` interface
4. **API Layer**:
   - Create controller in `Controllers/`

### CQRS Pattern Example

**Command (Write Operation):**
```csharp
public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int Stock
) : IRequest<Guid>;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    // Implementation
}
```

**Query (Read Operation):**
```csharp
public record GetProductsQuery : IRequest<List<ProductDto>>;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    // Implementation
}
```

### Running Tests

```bash
dotnet test
```

## 🔧 Configuration

### Database

The application uses **SQLite in-memory database** configured in `Infrastructure/DependencyInjection.cs`:

```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("DataSource=:memory:"));
```

**Note:** In-memory database is cleared on application restart. Seed data is automatically loaded on startup.

### Logging

Logging is configured using Serilog in `appsettings.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning"
      }
    }
  }
}
```

## 🚢 Deployment

### Build for Production

```bash
dotnet build --configuration Release
```

### Publish the Application

```bash
dotnet publish --configuration Release --output ./publish
```

### Docker (Optional)

Create a `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/AeInfinity.Api/AeInfinity.Api.csproj", "AeInfinity.Api/"]
COPY ["src/AeInfinity.Application/AeInfinity.Application.csproj", "AeInfinity.Application/"]
COPY ["src/AeInfinity.Infrastructure/AeInfinity.Infrastructure.csproj", "AeInfinity.Infrastructure/"]
COPY ["src/AeInfinity.Domain/AeInfinity.Domain.csproj", "AeInfinity.Domain/"]
RUN dotnet restore "AeInfinity.Api/AeInfinity.Api.csproj"
COPY . .
WORKDIR "/src/AeInfinity.Api"
RUN dotnet build "AeInfinity.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AeInfinity.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AeInfinity.Api.dll"]
```

## 📖 Resources

### Clean Architecture
- [Clean Architecture by Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Microsoft Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)

### Technologies
- [.NET 9.0 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [MediatR](https://github.com/jbogard/MediatR)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [AutoMapper](https://docs.automapper.org/)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is part of the AE Immersion Workshop.

## 👥 Authors

- **Reecha Kansal** - [rkSlalom](https://github.com/rkSlalom)

## 🔗 Links

- [Repository](https://github.com/rkSlalom/ae-infinity-api)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
