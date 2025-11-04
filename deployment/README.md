# Azure Deployment Guide for AE Infinity

This guide walks you through deploying the AE Infinity application to Azure.

## Prerequisites

1. **Azure Account**: [Create free account](https://azure.microsoft.com/free/)
2. **Azure CLI**: Install via `brew install azure-cli`
3. **Node.js & npm**: For frontend deployment
4. **.NET SDK**: Already installed for backend

## Quick Start

### 1. Install Azure CLI (if not installed)

```bash
brew install azure-cli
```

### 2. Login to Azure

```bash
az login
```

This will open a browser for authentication.

### 3. Verify Subscription

```bash
# List all subscriptions
az account list --output table

# Set the subscription you want to use
az account set --subscription "Your-Subscription-Name"
```

### 4. Create Azure Resources

```bash
cd deployment
chmod +x azure-setup.sh
./azure-setup.sh
```

This script will create:
- Resource Group
- App Service Plan
- Azure SQL Database
- App Service for Backend API
- Static Web App for Frontend

**⚠️ IMPORTANT**: Save the output! It contains your database credentials and URLs.

### 5. Update Code for Azure

Before deploying, you need to update a few configuration files:

#### Backend: Update Database Provider

Edit `src/AeInfinity.Infrastructure/DependencyInjection.cs`:

```csharp
// Replace SQLite configuration with SQL Server
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
```

Add SQL Server NuGet package:
```bash
cd src/AeInfinity.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

#### Frontend: Update API URL

The deploy script will create `.env.production` automatically with the correct API URL.

### 6. Deploy Backend API

```bash
chmod +x deploy-backend.sh
./deploy-backend.sh
```

This will:
- Build the .NET API
- Publish to Azure App Service
- Configure environment variables
- Set up CORS

### 7. Deploy Frontend

```bash
chmod +x deploy-frontend.sh
./deploy-frontend.sh
```

This will:
- Build the React app
- Deploy to Azure Static Web Apps
- Configure API endpoint

### 8. Initialize Database

After deploying the backend, the database will auto-initialize with seed data on first run.

To verify:
```bash
curl https://YOUR-API-NAME.azurewebsites.net/health
```

## Architecture

```
┌─────────────────────┐
│  Azure Static Web   │
│   Apps (Frontend)   │
│   React/Vite        │
└──────────┬──────────┘
           │
           │ HTTPS
           │
           ▼
┌─────────────────────┐
│   Azure App Service │
│   (Backend API)     │
│   .NET 8            │
└──────────┬──────────┘
           │
           │ SQL
           │
           ▼
┌─────────────────────┐
│  Azure SQL Database │
│   (Data Storage)    │
└─────────────────────┘
```

## Cost Estimates (Monthly)

- **App Service Plan (B1)**: ~$13/month
- **Azure SQL Database (S0)**: ~$15/month
- **Static Web App (Free tier)**: $0/month

**Total**: ~$28/month

### Free Tier Alternative

For development/testing, you can use:
- App Service Free (F1)
- SQL Database Basic (5 DTU)
- Static Web App Free

**Total**: ~$5/month

## Manual Steps via Azure Portal

If you prefer using the Azure Portal instead of CLI:

### 1. Create Resource Group
1. Go to [Azure Portal](https://portal.azure.com)
2. Search "Resource groups" → Create
3. Name: `rg-ae-infinity`
4. Region: Choose closest to you

### 2. Create SQL Database
1. Search "SQL databases" → Create
2. Create new server
3. Choose Basic tier for testing
4. Note credentials

### 3. Create App Service
1. Search "App Services" → Create
2. Runtime: .NET 8
3. OS: Linux
4. Pricing: B1 or F1 (free)

### 4. Create Static Web App
1. Search "Static Web Apps" → Create
2. Choose "Other" for deployment
3. Note deployment token

### 5. Deploy Manually

**Backend**:
```bash
cd src/AeInfinity.Api
dotnet publish -c Release -o ./publish
cd publish
zip -r ../api.zip .
# Upload via Azure Portal → App Service → Deployment Center
```

**Frontend**:
```bash
cd ae-infinity-ui
npm run build
# Upload via Azure Portal → Static Web App → Deployment
```

## Environment Variables

### Backend (App Service → Configuration → Application Settings)

| Key | Value | Description |
|-----|-------|-------------|
| `ConnectionStrings__DefaultConnection` | SQL connection string | Database connection |
| `ASPNETCORE_ENVIRONMENT` | `Production` | Environment |
| `Jwt__SecretKey` | Random secure key | JWT signing key |
| `Jwt__Issuer` | Your API URL | JWT issuer |
| `Jwt__Audience` | Your frontend URL | JWT audience |
| `Jwt__ExpirationInMinutes` | `1440` | Token expiration (24h) |

### Frontend (.env.production)

```env
VITE_API_URL=https://your-api-name.azurewebsites.net/api
```

## Troubleshooting

### Backend won't start
1. Check logs: Azure Portal → App Service → Log Stream
2. Verify connection string is correct
3. Check .NET version matches (8.0)

### Database connection fails
1. Verify firewall rules allow Azure services
2. Check connection string format
3. Verify credentials

### Frontend can't reach API
1. Check CORS configuration on backend
2. Verify API URL in `.env.production`
3. Check browser console for errors

### Static Web App deployment fails
1. Ensure build output is in `dist` folder
2. Check deployment token is valid
3. Try manual upload via Portal

## CI/CD Setup (Optional)

### Using GitHub Actions

Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]

jobs:
  deploy-backend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}
      - name: Deploy Backend
        run: |
          cd src/AeInfinity.Api
          dotnet publish -c Release -o ./publish
          # ... deployment steps

  deploy-frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-node@v3
      - name: Build and Deploy
        run: |
          cd ae-infinity-ui
          npm install
          npm run build
          # ... deployment steps
```

## Monitoring

### View Logs
```bash
# Backend logs
az webapp log tail --name $API_APP_NAME --resource-group $RESOURCE_GROUP

# Stream logs in browser
# Azure Portal → App Service → Log Stream
```

### Monitor Performance
- Azure Portal → App Service → Metrics
- Application Insights (optional, additional cost)

## Cleanup

To delete all Azure resources:

```bash
az group delete --name rg-ae-infinity --yes --no-wait
```

**⚠️ WARNING**: This will delete everything and cannot be undone!

## Security Best Practices

1. **Use Azure Key Vault** for secrets (production)
2. **Enable HTTPS only** (automatic on Azure)
3. **Configure Custom Domains** for production
4. **Enable Azure AD Authentication** (optional)
5. **Set up Application Insights** for monitoring
6. **Regular security updates** for dependencies

## Next Steps After Deployment

1. **Custom Domain**: Add your own domain
2. **SSL Certificate**: Azure provides free SSL
3. **Monitoring**: Set up Application Insights
4. **Backup**: Configure database backups
5. **Scaling**: Adjust App Service plan as needed
6. **CDN**: Add Azure CDN for better performance

## Support

- [Azure App Service Docs](https://docs.microsoft.com/azure/app-service/)
- [Azure Static Web Apps Docs](https://docs.microsoft.com/azure/static-web-apps/)
- [Azure SQL Database Docs](https://docs.microsoft.com/azure/sql-database/)

## Cost Optimization

1. Use **Free/Basic tiers** for development
2. Use **Auto-pause** for SQL Database in dev
3. Use **Deployment Slots** for zero-downtime updates
4. Set up **Auto-scaling** only if needed

