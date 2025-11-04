#!/bin/bash

# Azure Container Apps Deployment Setup Script
# Creates all necessary Azure resources for containerized deployment

# Configuration Variables
RESOURCE_GROUP="rg-ae-infinity-containers"
LOCATION="eastus"
ENVIRONMENT_NAME="ae-infinity-env"
ACR_NAME="acraeinfinityregistry$(openssl rand -hex 4)"  # Azure Container Registry
API_APP_NAME="api-ae-infinity"
WEB_APP_NAME="web-ae-infinity"
DB_SERVER_NAME="sql-ae-infinity-$(openssl rand -hex 4)"
DB_NAME="ae-infinity-db"
DB_ADMIN_USER="aeinfinityadmin"
DB_ADMIN_PASSWORD=""  # Will be generated
LOG_ANALYTICS_WORKSPACE="ae-infinity-logs"

echo "========================================="
echo "AE Infinity - Azure Container Apps Setup"
echo "========================================="
echo ""

# Check if required tools are installed
if ! command -v az &> /dev/null; then
    echo "❌ Azure CLI not found. Install it with: brew install azure-cli"
    exit 1
fi

if ! command -v docker &> /dev/null; then
    echo "❌ Docker not found. Install Docker Desktop for Mac"
    exit 1
fi

# Generate secure password for database
DB_ADMIN_PASSWORD=$(openssl rand -base64 32 | tr -d "=+/" | cut -c1-25)

echo "Configuration:"
echo "  Resource Group: $RESOURCE_GROUP"
echo "  Location: $LOCATION"
echo "  Container Registry: $ACR_NAME"
echo "  API App: $API_APP_NAME"
echo "  Web App: $WEB_APP_NAME"
echo "  DB Server: $DB_SERVER_NAME"
echo ""

# Step 1: Create Resource Group
echo "Step 1: Creating Resource Group..."
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION

# Step 2: Create Azure Container Registry
echo "Step 2: Creating Azure Container Registry..."
az acr create \
  --resource-group $RESOURCE_GROUP \
  --name $ACR_NAME \
  --sku Basic \
  --admin-enabled true

# Get ACR credentials
ACR_USERNAME=$(az acr credential show --name $ACR_NAME --query "username" -o tsv)
ACR_PASSWORD=$(az acr credential show --name $ACR_NAME --query "passwords[0].value" -o tsv)
ACR_LOGIN_SERVER=$(az acr show --name $ACR_NAME --query "loginServer" -o tsv)

# Step 3: Create Azure SQL Server
echo "Step 3: Creating Azure SQL Server..."
az sql server create \
  --name $DB_SERVER_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --admin-user $DB_ADMIN_USER \
  --admin-password "$DB_ADMIN_PASSWORD"

# Step 4: Configure SQL Server Firewall (Allow Azure Services)
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

# Step 6: Create Log Analytics Workspace
echo "Step 6: Creating Log Analytics Workspace..."
az monitor log-analytics workspace create \
  --resource-group $RESOURCE_GROUP \
  --workspace-name $LOG_ANALYTICS_WORKSPACE \
  --location $LOCATION

# Get Log Analytics workspace ID and key
LOG_ANALYTICS_WORKSPACE_ID=$(az monitor log-analytics workspace show \
  --resource-group $RESOURCE_GROUP \
  --workspace-name $LOG_ANALYTICS_WORKSPACE \
  --query customerId -o tsv)

LOG_ANALYTICS_KEY=$(az monitor log-analytics workspace get-shared-keys \
  --resource-group $RESOURCE_GROUP \
  --workspace-name $LOG_ANALYTICS_WORKSPACE \
  --query primarySharedKey -o tsv)

# Step 7: Create Container Apps Environment
echo "Step 7: Creating Container Apps Environment..."
az containerapp env create \
  --name $ENVIRONMENT_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --logs-workspace-id $LOG_ANALYTICS_WORKSPACE_ID \
  --logs-workspace-key $LOG_ANALYTICS_KEY

# Generate JWT secret
JWT_SECRET=$(openssl rand -base64 32)

# Build connection string
CONNECTION_STRING="Server=tcp:${DB_SERVER_NAME}.database.windows.net,1433;Initial Catalog=${DB_NAME};Persist Security Info=False;User ID=${DB_ADMIN_USER};Password=${DB_ADMIN_PASSWORD};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

echo ""
echo "========================================="
echo "✅ Azure Resources Created Successfully!"
echo "========================================="
echo ""
echo "📝 SAVE THESE CREDENTIALS (IMPORTANT!):"
echo ""
echo "Resource Group: $RESOURCE_GROUP"
echo "Container Registry: $ACR_LOGIN_SERVER"
echo "  Username: $ACR_USERNAME"
echo "  Password: $ACR_PASSWORD"
echo ""
echo "Database:"
echo "  Server: ${DB_SERVER_NAME}.database.windows.net"
echo "  Database: $DB_NAME"
echo "  Username: $DB_ADMIN_USER"
echo "  Password: $DB_ADMIN_PASSWORD"
echo ""
echo "JWT Secret: $JWT_SECRET"
echo ""
echo "Connection String:"
echo "$CONNECTION_STRING"
echo ""
echo "========================================="
echo "Next Steps:"
echo "1. Build and push Docker images: ./build-and-push.sh"
echo "2. Deploy containers: ./deploy-containers.sh"
echo "========================================="

# Save configuration to file
cat > azure-config.env << EOF
RESOURCE_GROUP=$RESOURCE_GROUP
LOCATION=$LOCATION
ENVIRONMENT_NAME=$ENVIRONMENT_NAME
ACR_NAME=$ACR_NAME
ACR_LOGIN_SERVER=$ACR_LOGIN_SERVER
ACR_USERNAME=$ACR_USERNAME
ACR_PASSWORD=$ACR_PASSWORD
API_APP_NAME=$API_APP_NAME
WEB_APP_NAME=$WEB_APP_NAME
DB_SERVER_NAME=$DB_SERVER_NAME
DB_NAME=$DB_NAME
DB_ADMIN_USER=$DB_ADMIN_USER
DB_ADMIN_PASSWORD=$DB_ADMIN_PASSWORD
JWT_SECRET=$JWT_SECRET
CONNECTION_STRING=$CONNECTION_STRING
LOG_ANALYTICS_WORKSPACE_ID=$LOG_ANALYTICS_WORKSPACE_ID
EOF

echo ""
echo "Configuration saved to: azure-config.env"
echo "⚠️  Keep this file secure - it contains sensitive credentials!"

