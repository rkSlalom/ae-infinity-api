#!/bin/bash

# Azure Deployment Setup Script for AE Infinity
# This script creates all necessary Azure resources

# Configuration Variables
RESOURCE_GROUP="rg-ae-infinity"
LOCATION="eastus"
APP_SERVICE_PLAN="asp-ae-infinity"
API_APP_NAME="api-ae-infinity-$(openssl rand -hex 4)"  # Adds random suffix for uniqueness
WEB_APP_NAME="web-ae-infinity"
DB_SERVER_NAME="sql-ae-infinity-$(openssl rand -hex 4)"
DB_NAME="ae-infinity-db"
DB_ADMIN_USER="aeinfinityadmin"
DB_ADMIN_PASSWORD=""  # Will be generated

echo "========================================="
echo "AE Infinity - Azure Deployment Setup"
echo "========================================="
echo ""

# Generate secure password for database
DB_ADMIN_PASSWORD=$(openssl rand -base64 32 | tr -d "=+/" | cut -c1-25)

echo "Configuration:"
echo "  Resource Group: $RESOURCE_GROUP"
echo "  Location: $LOCATION"
echo "  API App Name: $API_APP_NAME"
echo "  Web App Name: $WEB_APP_NAME"
echo "  DB Server: $DB_SERVER_NAME"
echo ""

# Step 1: Create Resource Group
echo "Step 1: Creating Resource Group..."
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION

# Step 2: Create App Service Plan (Linux, with support for .NET)
echo "Step 2: Creating App Service Plan..."
az appservice plan create \
  --name $APP_SERVICE_PLAN \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --is-linux \
  --sku B1

# Step 3: Create Azure SQL Server
echo "Step 3: Creating Azure SQL Server..."
az sql server create \
  --name $DB_SERVER_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --admin-user $DB_ADMIN_USER \
  --admin-password "$DB_ADMIN_PASSWORD"

# Step 4: Configure Firewall (Allow Azure Services)
echo "Step 4: Configuring SQL Server Firewall..."
az sql server firewall-rule create \
  --resource-group $RESOURCE_GROUP \
  --server $DB_SERVER_NAME \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Optional: Allow your local IP for testing
MY_IP=$(curl -s https://api.ipify.org)
az sql server firewall-rule create \
  --resource-group $RESOURCE_GROUP \
  --server $DB_SERVER_NAME \
  --name AllowMyIP \
  --start-ip-address $MY_IP \
  --end-ip-address $MY_IP

# Step 5: Create SQL Database
echo "Step 5: Creating SQL Database..."
az sql db create \
  --resource-group $RESOURCE_GROUP \
  --server $DB_SERVER_NAME \
  --name $DB_NAME \
  --service-objective S0 \
  --backup-storage-redundancy Local

# Step 6: Create App Service for Backend API
echo "Step 6: Creating App Service for Backend API..."
az webapp create \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_SERVICE_PLAN \
  --name $API_APP_NAME \
  --runtime "DOTNETCORE:8.0"

# Step 7: Create Static Web App for Frontend
echo "Step 7: Creating Static Web App for Frontend..."
az staticwebapp create \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku Free

# Step 8: Get URLs and Connection String
API_URL=$(az webapp show --resource-group $RESOURCE_GROUP --name $API_APP_NAME --query "defaultHostName" -o tsv)
WEB_URL=$(az staticwebapp show --name $WEB_APP_NAME --resource-group $RESOURCE_GROUP --query "defaultHostname" -o tsv)
CONNECTION_STRING="Server=tcp:${DB_SERVER_NAME}.database.windows.net,1433;Initial Catalog=${DB_NAME};Persist Security Info=False;User ID=${DB_ADMIN_USER};Password=${DB_ADMIN_PASSWORD};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

echo ""
echo "========================================="
echo "✅ Azure Resources Created Successfully!"
echo "========================================="
echo ""
echo "📝 SAVE THESE CREDENTIALS (IMPORTANT!):"
echo ""
echo "Resource Group: $RESOURCE_GROUP"
echo "API URL: https://$API_URL"
echo "Frontend URL: https://$WEB_URL"
echo ""
echo "Database Connection:"
echo "  Server: ${DB_SERVER_NAME}.database.windows.net"
echo "  Database: $DB_NAME"
echo "  Username: $DB_ADMIN_USER"
echo "  Password: $DB_ADMIN_PASSWORD"
echo ""
echo "Connection String (add to App Settings):"
echo "$CONNECTION_STRING"
echo ""
echo "========================================="
echo "Next Steps:"
echo "1. Update appsettings.json with connection string"
echo "2. Deploy backend: ./deploy-backend.sh"
echo "3. Deploy frontend: ./deploy-frontend.sh"
echo "========================================="

# Save configuration to file
cat > azure-config.env << EOF
RESOURCE_GROUP=$RESOURCE_GROUP
API_APP_NAME=$API_APP_NAME
WEB_APP_NAME=$WEB_APP_NAME
DB_SERVER_NAME=$DB_SERVER_NAME
DB_NAME=$DB_NAME
DB_ADMIN_USER=$DB_ADMIN_USER
DB_ADMIN_PASSWORD=$DB_ADMIN_PASSWORD
API_URL=https://$API_URL
WEB_URL=https://$WEB_URL
CONNECTION_STRING=$CONNECTION_STRING
EOF

echo ""
echo "Configuration saved to: azure-config.env"
echo "⚠️  Keep this file secure - it contains sensitive credentials!"

