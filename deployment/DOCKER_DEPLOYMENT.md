# 🐳 Docker + Azure Container Apps Deployment Guide

Complete guide for deploying AE Infinity using Docker containers to Azure Container Apps.

## Why Azure Container Apps?

✅ **More portable** - Run anywhere Docker runs  
✅ **Better scaling** - Auto-scale based on HTTP traffic, CPU, or memory  
✅ **Cost-effective** - Pay per request, scale to zero  
✅ **Modern** - Built on Kubernetes without the complexity  
✅ **Built-in features** - Load balancing, SSL, monitoring  

---

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│              Azure Container Apps Environment            │
│  ┌────────────────────────┐  ┌──────────────────────┐   │
│  │  Frontend Container    │  │  Backend Container   │   │
│  │  (Nginx + React)       │──│  (.NET 8 API)        │   │
│  │  Port 80               │  │  Port 8080           │   │
│  └────────────────────────┘  └──────────┬───────────┘   │
│                                          │               │
└──────────────────────────────────────────┼───────────────┘
                                           │
                                           ▼
                              ┌────────────────────────┐
                              │  Azure SQL Database    │
                              │  (Managed)             │
                              └────────────────────────┘
                                           ▲
                              ┌────────────────────────┐
                              │ Azure Container        │
                              │ Registry (ACR)         │
                              │ Stores Docker Images   │
                              └────────────────────────┘
```

---

## Prerequisites

### 1. Install Required Tools

```bash
# Azure CLI
brew install azure-cli

# Docker Desktop (macOS)
brew install --cask docker

# Verify installations
az --version
docker --version
```

### 2. Start Docker Desktop

Make sure Docker Desktop is running before proceeding.

### 3. Login to Azure

```bash
az login
az account set --subscription "Your-Subscription-Name"
```

---

## Local Testing with Docker Compose

Before deploying to Azure, test locally:

### 1. Start all services

```bash
cd /path/to/AE-Immersion-Workshop
docker-compose up --build
```

This starts:
- SQL Server on port 1433
- Backend API on port 8080
- Frontend UI on port 3000

### 2. Access the application

- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:8080/swagger
- **Health Check**: http://localhost:8080/health

### 3. Stop services

```bash
docker-compose down

# To remove volumes (database data):
docker-compose down -v
```

---

## Azure Deployment Steps

### Step 1: Create Azure Resources

```bash
cd ae-infinity-api/deployment
chmod +x *.sh
./azure-container-setup.sh
```

**⏱️ Duration**: ~10 minutes

This creates:
- ✅ Resource Group
- ✅ Azure Container Registry (ACR)
- ✅ Azure SQL Database
- ✅ Log Analytics Workspace
- ✅ Container Apps Environment

**⚠️ SAVE THE OUTPUT** - Contains credentials!

### Step 2: Update Database Configuration

The API currently uses SQLite. Update for SQL Server:

```bash
cd ../src/AeInfinity.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

Edit `DependencyInjection.cs`:

```csharp
// Replace this:
options.UseSqlite("DataSource=:memory:")

// With this:
options.UseSqlServer(
    configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
```

### Step 3: Build and Push Docker Images

```bash
cd ../../deployment
./build-and-push.sh
```

**⏱️ Duration**: ~5-10 minutes

This script:
1. Builds Backend API Docker image
2. Builds Frontend UI Docker image  
3. Tags images with timestamps
4. Pushes to Azure Container Registry

### Step 4: Deploy to Azure Container Apps

```bash
./deploy-containers.sh
```

**⏱️ Duration**: ~5 minutes

This deploys:
- Backend API container
- Frontend UI container
- Configures environment variables
- Sets up CORS
- Enables external ingress

### Step 5: Test Your Deployment

```bash
# Get URLs from output or:
source azure-config.env
echo "Frontend: $WEB_URL"
echo "Backend: $API_URL"

# Test API health
curl https://$API_FQDN/health
```

---

## Update Deployment

To deploy code changes:

```bash
# 1. Rebuild and push images
./build-and-push.sh

# 2. Update running containers
az containerapp update \
  --name api-ae-infinity \
  --resource-group rg-ae-infinity-containers \
  --image <ACR_LOGIN_SERVER>/ae-infinity-api:latest

az containerapp update \
  --name web-ae-infinity \
  --resource-group rg-ae-infinity-containers \
  --image <ACR_LOGIN_SERVER>/ae-infinity-ui:latest
```

Or use the convenience script:

```bash
./update-deployment.sh
```

---

## Monitoring & Management

### View Logs

```bash
# Real-time API logs
az containerapp logs show \
  --name api-ae-infinity \
  --resource-group rg-ae-infinity-containers \
  --follow

# Real-time Frontend logs
az containerapp logs show \
  --name web-ae-infinity \
  --resource-group rg-ae-infinity-containers \
  --follow
```

### Check Application Status

```bash
# API status
az containerapp show \
  --name api-ae-infinity \
  --resource-group rg-ae-infinity-containers \
  --query "properties.{Status:runningStatus,URL:configuration.ingress.fqdn}"

# Metrics
az monitor metrics list \
  --resource <container-app-resource-id> \
  --metric "Requests"
```

### Scale Configuration

```bash
# Manual scaling
az containerapp update \
  --name api-ae-infinity \
  --resource-group rg-ae-infinity-containers \
  --min-replicas 2 \
  --max-replicas 10

# Auto-scaling rules (HTTP)
az containerapp update \
  --name api-ae-infinity \
  --resource-group rg-ae-infinity-containers \
  --scale-rule-name http-rule \
  --scale-rule-type http \
  --scale-rule-http-concurrency 50
```

---

## Cost Optimization

### Development Environment

```yaml
Backend API:
  CPU: 0.25 vCPU
  Memory: 0.5 Gi
  Min replicas: 0  # Scale to zero
  Max replicas: 1

Frontend:
  CPU: 0.25 vCPU
  Memory: 0.5 Gi
  Min replicas: 0
  Max replicas: 1
```

**Estimated**: $5-10/month

### Production Environment

```yaml
Backend API:
  CPU: 0.5 vCPU
  Memory: 1.0 Gi
  Min replicas: 2  # High availability
  Max replicas: 10

Frontend:
  CPU: 0.5 vCPU
  Memory: 0.5 Gi
  Min replicas: 2
  Max replicas: 5
```

**Estimated**: $30-50/month (with SQL Database)

### Cost Saving Tips

1. **Scale to zero** in development:
```bash
az containerapp update \
  --name api-ae-infinity \
  --resource-group rg-ae-infinity-containers \
  --min-replicas 0
```

2. **Pause SQL Database** when not in use:
```bash
az sql db pause \
  --name ae-infinity-db \
  --server <sql-server-name> \
  --resource-group rg-ae-infinity-containers
```

3. **Use Basic ACR tier** for development
4. **Set budget alerts** in Azure Portal

---

## Troubleshooting

### Container won't start

```bash
# Check logs
az containerapp logs show -n api-ae-infinity -g rg-ae-infinity-containers --tail 100

# Check revision status
az containerapp revision list -n api-ae-infinity -g rg-ae-infinity-containers -o table
```

### Database connection fails

1. Check connection string in secrets:
```bash
az containerapp secret list -n api-ae-infinity -g rg-ae-infinity-containers
```

2. Verify SQL Server firewall:
```bash
az sql server firewall-rule list \
  --server <sql-server-name> \
  --resource-group rg-ae-infinity-containers
```

### Image pull fails

```bash
# Check registry credentials
az containerapp registry list -n api-ae-infinity -g rg-ae-infinity-containers

# Re-add registry credentials
az containerapp registry set \
  --name api-ae-infinity \
  --resource-group rg-ae-infinity-containers \
  --server <acr-login-server> \
  --username <acr-username> \
  --password <acr-password>
```

### CORS errors

Update CORS in backend:
```bash
az containerapp update \
  --name api-ae-infinity \
  --resource-group rg-ae-infinity-containers \
  --set-env-vars "Cors__AllowedOrigins=https://your-frontend-url"
```

---

## CI/CD with GitHub Actions

Create `.github/workflows/deploy-containers.yml`:

```yaml
name: Deploy to Azure Container Apps

on:
  push:
    branches: [ main ]

env:
  ACR_LOGIN_SERVER: ${{ secrets.ACR_LOGIN_SERVER }}
  RESOURCE_GROUP: rg-ae-infinity-containers

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Login to Azure
        uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}
      
      - name: Login to ACR
        run: |
          az acr login --name ${{ secrets.ACR_NAME }}
      
      - name: Build and Push Backend
        run: |
          cd ae-infinity-api
          docker build -t $ACR_LOGIN_SERVER/ae-infinity-api:${{ github.sha }} .
          docker push $ACR_LOGIN_SERVER/ae-infinity-api:${{ github.sha }}
      
      - name: Build and Push Frontend
        run: |
          cd ae-infinity-ui
          docker build --build-arg VITE_API_URL=${{ secrets.API_URL }} \
            -t $ACR_LOGIN_SERVER/ae-infinity-ui:${{ github.sha }} .
          docker push $ACR_LOGIN_SERVER/ae-infinity-ui:${{ github.sha }}
      
      - name: Deploy to Container Apps
        run: |
          az containerapp update \
            --name api-ae-infinity \
            --resource-group $RESOURCE_GROUP \
            --image $ACR_LOGIN_SERVER/ae-infinity-api:${{ github.sha }}
          
          az containerapp update \
            --name web-ae-infinity \
            --resource-group $RESOURCE_GROUP \
            --image $ACR_LOGIN_SERVER/ae-infinity-ui:${{ github.sha }}
```

---

## Security Best Practices

1. **Use Managed Identity** (instead of passwords):
```bash
az containerapp identity assign \
  --name api-ae-infinity \
  --resource-group rg-ae-infinity-containers \
  --system-assigned
```

2. **Store secrets in Azure Key Vault**:
```bash
az keyvault create \
  --name ae-infinity-vault \
  --resource-group rg-ae-infinity-containers \
  --location eastus
```

3. **Enable HTTPS only** (automatic in Container Apps)

4. **Use Virtual Network** for private communication:
```bash
az containerapp env create \
  --name ae-infinity-env \
  --resource-group rg-ae-infinity-containers \
  --internal-only true
```

5. **Regular security updates**:
```bash
# Rebuild images regularly
./build-and-push.sh
```

---

## Cleanup

To delete all Azure resources:

```bash
az group delete --name rg-ae-infinity-containers --yes --no-wait
```

⚠️ **WARNING**: This deletes everything including data!

---

## Comparison: Container Apps vs App Service

| Feature | Container Apps | App Service |
|---------|---------------|-------------|
| **Container Support** | Native | Docker support |
| **Scaling** | Scale to zero | Minimum 1 instance |
| **Pricing** | Per-request | Per-hour |
| **Auto-scale** | HTTP, CPU, Memory, Events | CPU, Memory only |
| **Microservices** | Excellent | Good |
| **Setup Complexity** | Medium | Simple |
| **Best For** | Modern containerized apps | Traditional web apps |

---

## Support & Resources

- [Azure Container Apps Docs](https://docs.microsoft.com/azure/container-apps/)
- [Docker Documentation](https://docs.docker.com/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
- [Nginx Docker Images](https://hub.docker.com/_/nginx)

---

## Quick Command Reference

```bash
# Local testing
docker-compose up --build
docker-compose down -v

# Azure deployment
./azure-container-setup.sh       # Setup (once)
./build-and-push.sh              # Build & push images
./deploy-containers.sh           # Deploy to Azure

# Management
az containerapp logs show -n api-ae-infinity -g rg-ae-infinity-containers --follow
az containerapp revision list -n api-ae-infinity -g rg-ae-infinity-containers
az containerapp update -n api-ae-infinity -g rg-ae-infinity-containers --image <new-image>

# Cleanup
az group delete --name rg-ae-infinity-containers --yes
```

---

**🎉 Congratulations!** Your application is now running in a modern, containerized environment on Azure!

