# 🚀 Azure Deployment Checklist

Use this checklist to ensure you complete all deployment steps.

## Pre-Deployment

- [ ] Azure account created/accessed
- [ ] Azure CLI installed (`brew install azure-cli`)
- [ ] Logged into Azure (`az login`)
- [ ] Subscription selected (`az account set`)
- [ ] Code is working locally (backend on 5233, frontend on 5174)

## Database Migration

Current: SQLite in-memory → Target: Azure SQL Database

- [ ] Install SQL Server EF Core package
  ```bash
  cd src/AeInfinity.Infrastructure
  dotnet add package Microsoft.EntityFrameworkCore.SqlServer
  ```

- [ ] Update `DependencyInjection.cs` to use SQL Server
- [ ] Test connection string format
- [ ] Verify migrations work with SQL Server

## Azure Resource Creation

- [ ] Run `./azure-setup.sh`
- [ ] Save output credentials to safe location
- [ ] Verify all resources created in Azure Portal
- [ ] Test database connection

## Code Updates

### Backend
- [ ] Connection string configured for SQL Server
- [ ] CORS settings include frontend URL
- [ ] JWT settings configured
- [ ] Environment variables set
- [ ] Health check endpoint working

### Frontend
- [ ] `.env.production` created with API URL
- [ ] API client configured for production
- [ ] CORS headers tested
- [ ] Error handling for production

## Deployment

- [ ] Backend deployed: `./deploy-backend.sh`
- [ ] Backend health check: `curl https://YOUR-API.azurewebsites.net/health`
- [ ] Swagger accessible: `https://YOUR-API.azurewebsites.net/swagger`
- [ ] Frontend deployed: `./deploy-frontend.sh`
- [ ] Frontend accessible in browser

## Testing

- [ ] Login functionality works
- [ ] Can create a new list
- [ ] Can add items to list
- [ ] Can mark items as purchased
- [ ] Can share list with another user
- [ ] All CRUD operations working

## Security

- [ ] HTTPS enabled (automatic)
- [ ] CORS properly configured
- [ ] Database firewall rules set
- [ ] Secrets not committed to git
- [ ] JWT secret key is secure

## Optional Enhancements

- [ ] Custom domain configured
- [ ] Application Insights enabled
- [ ] Database backup configured
- [ ] CI/CD pipeline setup
- [ ] Monitoring alerts configured

## Post-Deployment

- [ ] Document URLs for team
- [ ] Save credentials securely (Azure Key Vault)
- [ ] Test all user workflows
- [ ] Monitor logs for errors
- [ ] Set up regular backups

## Rollback Plan

If something goes wrong:

1. Check logs: `az webapp log tail --name API_APP_NAME --resource-group RESOURCE_GROUP`
2. Revert to previous deployment via Azure Portal
3. Check environment variables in App Service
4. Verify database connection
5. Test endpoints with Postman/curl

## Cost Monitoring

- [ ] Set up cost alerts in Azure Portal
- [ ] Review pricing tier selections
- [ ] Monitor database DTU usage
- [ ] Check API request volume

## Troubleshooting Resources

- Deployment logs: Azure Portal → App Service → Deployment Center
- Application logs: Azure Portal → App Service → Log Stream
- Database metrics: Azure Portal → SQL Database → Metrics
- Static Web App: Azure Portal → Static Web App → Functions

---

**Estimated Time**: 2-3 hours for first deployment

**Support**: See README.md for detailed steps and troubleshooting

