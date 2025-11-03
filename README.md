# AE Infinity API

A modern .NET 9.0 Web API for a **collaborative shopping list application** built with **Clean Architecture** principles, CQRS pattern, and Entity Framework Core with SQLite in-memory database.

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

### Application Features
- ✅ **User Authentication** - JWT-based authentication with secure login/logout
- ✅ **Shopping List Management** - Create, read, update, delete, and archive lists
- ✅ **Collaborative Lists** - Share lists with role-based permissions (Owner, Editor, Viewer)
- ✅ **List Items** - Full CRUD operations with purchase tracking
- ✅ **Category Organization** - 10 predefined categories for organizing items
- ✅ **Soft Delete** - All records use soft delete for audit trail and recovery
- ✅ **Comprehensive Audit Trail** - Track who created, modified, and deleted records

### Architecture & Design Patterns
- ✅ **Clean Architecture** - Separation of concerns with clear boundaries
- ✅ **CQRS Pattern** - Using MediatR for command/query separation
- ✅ **Repository Pattern** - Data access abstraction
- ✅ **Dependency Injection** - Built-in ASP.NET Core DI container
- ✅ **Result Pattern** - Functional error handling

### Technologies & Frameworks
- ✅ **.NET 9.0** - Latest .NET framework
- ✅ **Entity Framework Core 9.0** - ORM with Code First approach
- ✅ **SQLite In-Memory Database** - Fast, lightweight data storage with persistent connection
- ✅ **MediatR** - CQRS and mediator pattern implementation
- ✅ **FluentValidation** - Input validation with fluent API
- ✅ **AutoMapper** - Object-to-object mapping
- ✅ **JWT Authentication** - Secure token-based authentication
- ✅ **BCrypt.Net-Next** - Password hashing
- ✅ **Swagger/OpenAPI** - Interactive API documentation
- ✅ **Serilog** - Structured logging

### API Features
- ✅ RESTful API endpoints
- ✅ JWT Bearer token authentication
- ✅ Role-based authorization
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

### 5. Default Test Users

The database is automatically seeded with test users:

| Email | Password | Description |
|-------|----------|-------------|
| sarah@example.com | Password123! | List owner with sample data |
| alex@example.com | Password123! | Collaborator |
| mike@example.com | Password123! | Collaborator |

## 📁 Project Structure

```
ae-infinity-api/
├── docs/                                    # Documentation
│   ├── DB_SCHEMA.md                        # Database schema specification
│   ├── API_LIST.md                         # Complete API endpoint list
│   └── IMPLEMENTATION_PLAN.md              # Development roadmap
│
├── src/
│   ├── AeInfinity.Domain/                  # Core Domain Layer
│   │   ├── Common/                         # Base entities and interfaces
│   │   │   ├── BaseEntity.cs
│   │   │   └── BaseAuditableEntity.cs
│   │   ├── Entities/                       # Domain entities
│   │   │   ├── User.cs
│   │   │   ├── Role.cs
│   │   │   ├── List.cs
│   │   │   ├── ListItem.cs
│   │   │   ├── Category.cs
│   │   │   └── UserToList.cs
│   │   └── Exceptions/                     # Domain exceptions
│   │       ├── NotFoundException.cs
│   │       ├── UnauthorizedException.cs
│   │       ├── ValidationException.cs
│   │       └── ForbiddenException.cs
│   │
│   ├── AeInfinity.Application/             # Application Layer
│   │   ├── Common/
│   │   │   ├── Behaviors/                  # MediatR pipeline behaviors
│   │   │   │   └── ValidationBehavior.cs
│   │   │   ├── Interfaces/                 # Application interfaces
│   │   │   │   ├── IApplicationDbContext.cs
│   │   │   │   └── IRepository.cs
│   │   │   ├── Mappings/                   # AutoMapper profiles
│   │   │   │   └── MappingProfile.cs
│   │   │   └── Models/
│   │   │       ├── DTOs/                   # Data Transfer Objects
│   │   │       │   ├── UserDto.cs
│   │   │       │   ├── ListDto.cs
│   │   │       │   ├── ListItemDto.cs
│   │   │       │   ├── CategoryDto.cs
│   │   │       │   ├── RoleDto.cs
│   │   │       │   └── CollaboratorDto.cs
│   │   │       └── Result.cs               # Result pattern
│   │   ├── Features/                       # Feature-based organization
│   │   │   ├── Auth/                       # Authentication
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── Login/
│   │   │   │   │   │   ├── LoginCommand.cs
│   │   │   │   │   │   ├── LoginCommandHandler.cs
│   │   │   │   │   │   └── LoginCommandValidator.cs
│   │   │   │   │   └── Logout/
│   │   │   │   │       ├── LogoutCommand.cs
│   │   │   │   │       └── LogoutCommandHandler.cs
│   │   │   └── Users/                      # User management
│   │   │       └── Queries/
│   │   │           └── GetCurrentUser/
│   │   │               ├── GetCurrentUserQuery.cs
│   │   │               └── GetCurrentUserQueryHandler.cs
│   │   └── DependencyInjection.cs          # Service registration
│   │
│   ├── AeInfinity.Infrastructure/          # Infrastructure Layer
│   │   ├── Persistence/
│   │   │   ├── Configurations/             # EF Core entity configurations
│   │   │   │   ├── UserConfiguration.cs
│   │   │   │   ├── RoleConfiguration.cs
│   │   │   │   ├── ListConfiguration.cs
│   │   │   │   ├── ListItemConfiguration.cs
│   │   │   │   ├── CategoryConfiguration.cs
│   │   │   │   └── UserToListConfiguration.cs
│   │   │   ├── Repositories/               # Repository implementations
│   │   │   │   └── Repository.cs
│   │   │   ├── ApplicationDbContext.cs     # DbContext
│   │   │   ├── DbInitializer.cs            # Database initialization
│   │   │   └── DbSeeder.cs                 # Seed data
│   │   ├── Services/                       # Infrastructure services
│   │   │   ├── JwtTokenService.cs          # JWT token generation
│   │   │   └── PasswordHasher.cs           # Password hashing (BCrypt)
│   │   └── DependencyInjection.cs          # Service registration
│   │
│   └── AeInfinity.Api/                     # Presentation Layer
│       ├── Controllers/                    # API Controllers
│       │   ├── BaseApiController.cs
│       │   ├── AuthController.cs
│       │   └── UsersController.cs
│       ├── Middleware/                     # Custom middleware
│       │   └── ExceptionHandlingMiddleware.cs
│       ├── Extensions/                     # Extension methods
│       │   └── WebApplicationExtensions.cs
│       ├── Properties/
│       │   └── launchSettings.json
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── AeInfinity.Api.http             # HTTP request samples
│       ├── test-auth.http                  # Auth test requests
│       └── Program.cs                      # Application entry point
│
├── .cursorrules                            # AI agent development rules
├── AeInfinity.sln                          # Solution file
├── .editorconfig                           # Code style configuration
├── .gitignore                              # Git ignore rules
└── README.md                               # This file
```

## 📚 API Documentation

### Swagger UI

Once running, access the interactive API documentation at:
- **Swagger UI**: http://localhost:5233/

### Authentication

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "sarah@example.com",
  "password": "Password123!"
}
```

**Response:** `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
    "email": "sarah@example.com",
    "displayName": "Sarah Johnson"
  }
}
```

#### Logout
```http
POST /api/auth/logout
Authorization: Bearer {token}
```

**Response:** `204 No Content`

### User Endpoints

#### Get Current User
```http
GET /api/users/me
Authorization: Bearer {token}
```

**Response:** `200 OK`
```json
{
  "id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
  "email": "sarah@example.com",
  "displayName": "Sarah Johnson",
  "avatarUrl": null,
  "isEmailVerified": true,
  "lastLoginAt": "2025-11-03T20:35:35.746786",
  "createdAt": "2025-11-03T20:35:22.33116"
}
```

### Planned Endpoints

See [docs/API_LIST.md](docs/API_LIST.md) for the complete API specification including:
- Shopping Lists (CRUD, archive, collaboration)
- List Items (CRUD, purchase tracking)
- List Sharing (invitations, collaborators, permissions)
- Search (lists and items)
- Statistics and history

### Validation

All requests are validated using FluentValidation. Invalid requests return `400 Bad Request` with detailed error information:

```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "errors": [
    {
      "property": "Email",
      "message": "Email is required."
    },
    {
      "property": "Password",
      "message": "Password must be at least 8 characters."
    }
  ]
}
```

## 🗄️ Database Schema

The application uses 6 main tables:

1. **users** - User accounts and authentication
2. **roles** - Permission levels (Owner, Editor, Editor-Limited, Viewer)
3. **lists** - Shopping lists
4. **list_items** - Items within lists
5. **categories** - 10 predefined categories (Produce, Dairy, Meat, etc.)
6. **user_to_list** - List collaboration and permissions

All tables implement:
- **Soft Delete** - Records are never physically deleted
- **Audit Trail** - Tracks created_by, created_at, modified_by, modified_at, deleted_by, deleted_at
- **GUID Primary Keys**

For detailed schema information, see [docs/DB_SCHEMA.md](docs/DB_SCHEMA.md).

### Seed Data

The database is automatically seeded on startup with:
- **4 Default Roles**: Owner, Editor, Editor-Limited, Viewer
- **10 Default Categories**: Produce, Dairy, Meat, Bakery, Beverages, Snacks, Frozen, Household, Personal Care, Other
- **3 Test Users**: Sarah (list owner), Alex, Mike (collaborators)
- **Sample Lists and Items**: Pre-populated data for testing

## 🏗️ Development Guidelines

### Adding a New Feature

Follow the **feature development workflow** defined in `.cursorrules`:

1. **Domain Layer First**
   - Create entity in `Domain/Entities/`
   - Add domain exceptions if needed
   - Define value objects if appropriate

2. **Application Layer Second**
   - Create feature folder in `Application/Features/{FeatureName}/`
   - Add Commands with handlers and validators
   - Add Queries with handlers and DTOs
   - Update `MappingProfile.cs`

3. **Infrastructure Layer Third**
   - Add entity configuration in `Persistence/Configurations/`
   - Update `IApplicationDbContext` interface
   - Add `DbSet<>` to `ApplicationDbContext`
   - Implement any infrastructure services needed

4. **API Layer Last**
   - Create controller in `Controllers/`
   - Add endpoints with proper attributes
   - Add XML documentation
   - Test endpoints

5. **Verify**
   - Build solution
   - Test all endpoints
   - Update README if needed
   - Check no architecture violations

### CQRS Pattern Example

**Command (Write Operation):**
```csharp
public class LoginCommand : IRequest<LoginResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    // Implementation with validation, authentication, JWT generation
}
```

**Query (Read Operation):**
```csharp
public class GetCurrentUserQuery : IRequest<UserDto>
{
    public Guid UserId { get; set; }
}

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    // Implementation with database query and DTO mapping
}
```

### Running Tests

```bash
dotnet test
```

## 🔧 Configuration

### Database

The application uses **SQLite in-memory database** with persistent connection configured in `Infrastructure/DependencyInjection.cs`:

```csharp
var keepAliveConnection = new SqliteConnection("DataSource=InMemoryDb;Mode=Memory;Cache=Shared");
keepAliveConnection.Open();
services.AddSingleton(keepAliveConnection);

services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(keepAliveConnection));
```

**Note:** In-memory database persists during application runtime but is cleared on restart. Seed data is automatically reloaded on startup.

### JWT Authentication

Configure JWT settings in `appsettings.json`:

```json
{
  "Jwt": {
    "Secret": "your-secret-key-min-32-characters-long",
    "Issuer": "AeInfinity",
    "Audience": "AeInfinityUsers",
    "ExpirationMinutes": 60
  }
}
```

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

## 📖 Documentation

### Project Documentation
- [Database Schema](docs/DB_SCHEMA.md) - Complete database schema with tables, relationships, and constraints
- [API Endpoint List](docs/API_LIST.md) - All API endpoints with authorization matrix
- [Implementation Plan](docs/IMPLEMENTATION_PLAN.md) - Development phases and progress tracking
- [AI Agent Rules](.cursorrules) - Development guidelines and architecture rules

### External Resources

#### Clean Architecture
- [Clean Architecture by Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Microsoft Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)

#### Technologies
- [.NET 9.0 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [MediatR](https://github.com/jbogard/MediatR)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [AutoMapper](https://docs.automapper.org/)
- [BCrypt.Net](https://github.com/BcryptNet/bcrypt.net)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Follow the architecture rules defined in `.cursorrules`
4. Commit your changes (`git commit -m 'Add some amazing feature'`)
5. Push to the branch (`git push origin feature/amazing-feature`)
6. Open a Pull Request

## 📝 License

This project is part of the AE Immersion Workshop.

## 👥 Authors

- **Reecha Kansal** - [rkSlalom](https://github.com/rkSlalom)

## 🔗 Links

- [Repository](https://github.com/rkSlalom/ae-infinity-api)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [AE Infinity Context Repository](../ae-infinity-context/) - Project specifications and requirements

---

**Status:** ✅ Phase 6 Complete - Authentication endpoints (login, logout, get current user) fully implemented and tested.
