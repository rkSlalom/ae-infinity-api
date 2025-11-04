# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/AeInfinity.Api/AeInfinity.Api.csproj", "AeInfinity.Api/"]
COPY ["src/AeInfinity.Application/AeInfinity.Application.csproj", "AeInfinity.Application/"]
COPY ["src/AeInfinity.Domain/AeInfinity.Domain.csproj", "AeInfinity.Domain/"]
COPY ["src/AeInfinity.Infrastructure/AeInfinity.Infrastructure.csproj", "AeInfinity.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "AeInfinity.Api/AeInfinity.Api.csproj"

# Copy all source code
COPY src/ .

# Build the application
WORKDIR "/src/AeInfinity.Api"
RUN dotnet build "AeInfinity.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "AeInfinity.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create a non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published app
COPY --from=publish /app/publish .

# Change ownership
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "AeInfinity.Api.dll"]

