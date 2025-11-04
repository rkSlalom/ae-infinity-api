#!/bin/bash

# Deploy Container Apps Script
# Deploys both backend and frontend container apps to Azure

# Load configuration
if [ -f "azure-config.env" ]; then
    source azure-config.env
else
    echo "❌ Error: azure-config.env not found. Run azure-container-setup.sh first."
    exit 1
fi

echo "========================================="
echo "Deploying Container Apps to Azure"
echo "========================================="
echo ""
echo "Environment: $ENVIRONMENT_NAME"
echo "Resource Group: $RESOURCE_GROUP"
echo ""

# Step 1: Deploy Backend API Container App
echo "Step 1: Deploying Backend API Container App..."
az containerapp create \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --environment $ENVIRONMENT_NAME \
  --image $ACR_LOGIN_SERVER/ae-infinity-api:latest \
  --registry-server $ACR_LOGIN_SERVER \
  --registry-username $ACR_USERNAME \
  --registry-password $ACR_PASSWORD \
  --target-port 8080 \
  --ingress external \
  --min-replicas 1 \
  --max-replicas 3 \
  --cpu 0.5 \
  --memory 1.0Gi \
  --secrets \
    connection-string="$CONNECTION_STRING" \
    jwt-secret="$JWT_SECRET" \
  --env-vars \
    "ASPNETCORE_ENVIRONMENT=Production" \
    "ASPNETCORE_URLS=http://+:8080" \
    "ConnectionStrings__DefaultConnection=secretref:connection-string" \
    "Jwt__SecretKey=secretref:jwt-secret" \
    "Jwt__Issuer=https://${API_APP_NAME}.${LOCATION}.azurecontainerapps.io" \
    "Jwt__Audience=https://${WEB_APP_NAME}.${LOCATION}.azurecontainerapps.io" \
    "Jwt__ExpirationInMinutes=1440"

# Get API FQDN
API_FQDN=$(az containerapp show \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --query properties.configuration.ingress.fqdn -o tsv)

echo "✅ Backend API deployed to: https://$API_FQDN"

# Step 2: Deploy Frontend UI Container App
echo "Step 2: Deploying Frontend UI Container App..."
az containerapp create \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --environment $ENVIRONMENT_NAME \
  --image $ACR_LOGIN_SERVER/ae-infinity-ui:latest \
  --registry-server $ACR_LOGIN_SERVER \
  --registry-username $ACR_USERNAME \
  --registry-password $ACR_PASSWORD \
  --target-port 80 \
  --ingress external \
  --min-replicas 1 \
  --max-replicas 3 \
  --cpu 0.25 \
  --memory 0.5Gi

# Get Web FQDN
WEB_FQDN=$(az containerapp show \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --query properties.configuration.ingress.fqdn -o tsv)

echo "✅ Frontend UI deployed to: https://$WEB_FQDN"

# Step 3: Configure CORS on Backend
echo "Step 3: Updating Backend CORS configuration..."
az containerapp update \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --set-env-vars \
    "Cors__AllowedOrigins=https://$WEB_FQDN,http://localhost:3000,http://localhost:5173,http://localhost:5174"

echo ""
echo "========================================="
echo "✅ Deployment Complete!"
echo "========================================="
echo ""
echo "🌐 Application URLs:"
echo "  Frontend: https://$WEB_FQDN"
echo "  Backend API: https://$API_FQDN"
echo "  API Docs: https://$API_FQDN/swagger"
echo ""
echo "🔍 Monitor logs:"
echo "  Backend: az containerapp logs show -n $API_APP_NAME -g $RESOURCE_GROUP --follow"
echo "  Frontend: az containerapp logs show -n $WEB_APP_NAME -g $RESOURCE_GROUP --follow"
echo ""
echo "📊 View metrics:"
echo "  az containerapp show -n $API_APP_NAME -g $RESOURCE_GROUP"
echo ""
echo "🔄 Update deployment:"
echo "  1. Rebuild images: ./build-and-push.sh"
echo "  2. Redeploy: az containerapp update -n $API_APP_NAME -g $RESOURCE_GROUP --image $ACR_LOGIN_SERVER/ae-infinity-api:latest"
echo ""

# Save URLs to config
cat >> azure-config.env << EOF
API_FQDN=$API_FQDN
WEB_FQDN=$WEB_FQDN
API_URL=https://$API_FQDN
WEB_URL=https://$WEB_FQDN
EOF

echo "URLs saved to azure-config.env"

