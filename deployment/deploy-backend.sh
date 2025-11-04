#!/bin/bash

# Backend API Deployment Script
# Deploys .NET API to Azure App Service

# Load configuration
if [ -f "azure-config.env" ]; then
    source azure-config.env
else
    echo "❌ Error: azure-config.env not found. Run azure-setup.sh first."
    exit 1
fi

echo "========================================="
echo "Deploying Backend API to Azure"
echo "========================================="
echo ""
echo "Target: $API_APP_NAME"
echo "Resource Group: $RESOURCE_GROUP"
echo ""

# Navigate to API project
cd ../src/AeInfinity.Api

# Step 1: Configure App Settings (Environment Variables)
echo "Step 1: Configuring App Settings..."
az webapp config appsettings set \
  --resource-group $RESOURCE_GROUP \
  --name $API_APP_NAME \
  --settings \
    ConnectionStrings__DefaultConnection="$CONNECTION_STRING" \
    ASPNETCORE_ENVIRONMENT="Production" \
    Jwt__SecretKey="$(openssl rand -base64 32)" \
    Jwt__Issuer="https://$API_APP_NAME.azurewebsites.net" \
    Jwt__Audience="https://$WEB_URL" \
    Jwt__ExpirationInMinutes="1440"

# Step 2: Build and Publish the API
echo "Step 2: Building and Publishing API..."
dotnet publish -c Release -o ./publish

# Step 3: Create deployment package
echo "Step 3: Creating deployment package..."
cd publish
zip -r ../api-deploy.zip .
cd ..

# Step 4: Deploy to Azure
echo "Step 4: Deploying to Azure App Service..."
az webapp deployment source config-zip \
  --resource-group $RESOURCE_GROUP \
  --name $API_APP_NAME \
  --src api-deploy.zip

# Step 5: Configure CORS
echo "Step 5: Configuring CORS..."
az webapp cors add \
  --resource-group $RESOURCE_GROUP \
  --name $API_APP_NAME \
  --allowed-origins "https://$WEB_URL" "http://localhost:5173" "http://localhost:5174"

# Cleanup
rm -rf publish api-deploy.zip

echo ""
echo "========================================="
echo "✅ Backend API Deployed Successfully!"
echo "========================================="
echo ""
echo "API URL: https://$API_APP_NAME.azurewebsites.net"
echo "Swagger: https://$API_APP_NAME.azurewebsites.net/swagger"
echo ""
echo "Testing API:"
echo "  curl https://$API_APP_NAME.azurewebsites.net/health"
echo ""

