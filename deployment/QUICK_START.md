# ⚡ Quick Start - Azure Deployment

Get your app deployed to Azure in ~15 minutes.

## 1️⃣ Install Azure CLI

```bash
brew install azure-cli
```

## 2️⃣ Login to Azure

```bash
az login
```

## 3️⃣ Run Setup Script

```bash
cd ae-infinity-api/deployment
./azure-setup.sh
```

**⚠️ SAVE THE OUTPUT** - It contains your database credentials!

## 4️⃣ Update Database Configuration

Edit `src/AeInfinity.Infrastructure/DependencyInjection.cs`:

**Before:**
```csharp
// SQLite in-memory
options.UseSqlite("DataSource=:memory:")
```

**After:**
```csharp
// Azure SQL Database
options.UseSqlServer(
    configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
```

Add package:
```bash
cd src/AeInfinity.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

## 5️⃣ Deploy Backend

```bash
cd ../../deployment
./deploy-backend.sh
```

## 6️⃣ Deploy Frontend

```bash
./deploy-frontend.sh
```

## 7️⃣ Test Your App

Visit the URLs from the setup script output:
- **Frontend**: `https://web-ae-infinity.azurewebsites.net`
- **API**: `https://api-ae-infinity-XXXX.azurewebsites.net/swagger`

## 🎉 Done!

Your app is now live on Azure.

---

## Troubleshooting

### "Port already in use" error
Kill existing processes:
```bash
pkill -f dotnet
pkill -f vite
```

### Backend won't start
Check logs:
```bash
az webapp log tail --name YOUR-API-NAME --resource-group rg-ae-infinity
```

### Database connection fails
Verify in Azure Portal:
1. SQL Server → Networking → Allow Azure services

### Frontend can't reach API
Check CORS in backend App Settings:
```bash
az webapp cors show --name YOUR-API-NAME --resource-group rg-ae-infinity
```

---

## Cost

**Development tier**: ~$5-10/month
**Production tier**: ~$28/month

Stop costs when not in use:
```bash
az webapp stop --name YOUR-API-NAME --resource-group rg-ae-infinity
az sql db pause --name ae-infinity-db --server YOUR-SQL-SERVER --resource-group rg-ae-infinity
```

---

## Clean Up

Delete everything:
```bash
az group delete --name rg-ae-infinity --yes
```

⚠️ This deletes ALL resources and data!

---

## Next Steps

- [ ] Set up custom domain
- [ ] Enable monitoring
- [ ] Configure CI/CD
- [ ] Add Application Insights

See `README.md` for detailed documentation.

