# AE Infinity API

A modern .NET 9.0 Web API backend service for the AE Infinity project.

## 🚀 Features

- **ASP.NET Core 9.0** - Latest .NET framework
- **OpenAPI/Swagger** - Built-in API documentation
- **Minimal APIs** - Clean and lightweight endpoint definitions
- **HTTPS Support** - Secure communication
- **Development Environment** - Pre-configured for local development

## 📋 Prerequisites

Before you begin, ensure you have the following installed:
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- [Git](https://git-scm.com/)
- A code editor (Visual Studio, VS Code, or Rider)

## 🛠️ Getting Started

### Clone the Repository

```bash
git clone https://github.com/rkSlalom/ae-infinity-api.git
cd ae-infinity-api
```

### Restore Dependencies

```bash
dotnet restore
```

### Build the Project

```bash
dotnet build
```

### Run the Application

```bash
cd ae-infinity-api
dotnet run
```

The API will be available at:
- **HTTP**: http://localhost:5233
- **HTTPS**: https://localhost:7184

## 📚 API Documentation

Once the application is running, you can access the OpenAPI specification:
- **OpenAPI JSON**: http://localhost:5233/openapi/v1.json

## 🧪 Sample Endpoints

### Weather Forecast
```http
GET http://localhost:5233/weatherforecast
```

Returns a 5-day weather forecast with random temperature data.

**Response Example:**
```json
[
  {
    "date": "2025-11-04",
    "temperatureC": 15,
    "temperatureF": 59,
    "summary": "Cool"
  }
]
```

## 🏗️ Project Structure

```
ae-infinity-api/
├── ae-infinity-api/
│   ├── Program.cs                    # Application entry point and configuration
│   ├── ae-infinity-api.csproj       # Project file
│   ├── ae-infinity-api.http         # HTTP request samples
│   ├── appsettings.json             # Application configuration
│   ├── appsettings.Development.json # Development configuration
│   └── Properties/
│       └── launchSettings.json      # Launch profiles
├── ae-infinity-api.sln              # Solution file
├── .gitignore                       # Git ignore rules
└── README.md                        # This file
```

## 🔧 Configuration

Application settings can be modified in:
- `appsettings.json` - Production settings
- `appsettings.Development.json` - Development-specific settings

## 🚢 Deployment

### Build for Production

```bash
dotnet build --configuration Release
```

### Publish the Application

```bash
dotnet publish --configuration Release --output ./publish
```

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
- [.NET Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/)

