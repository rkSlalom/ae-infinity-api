#!/bin/bash

# Quick Update Script - Rebuild and redeploy containers

# Load configuration
if [ -f "azure-config.env" ]; then
    source azure-config.env
else
    echo "❌ Error: azure-config.env not found."
    exit 1
fi

echo "========================================="
echo "Updating Container Apps Deployment"
echo "========================================="
echo ""

# Rebuild and push images
echo "Step 1: Building and pushing new images..."
./build-and-push.sh

if [ $? -ne 0 ]; then
    echo "❌ Build failed. Aborting deployment."
    exit 1
fi

# Update backend
echo ""
echo "Step 2: Updating Backend API..."
az containerapp update \
  --name $API_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --image $ACR_LOGIN_SERVER/ae-infinity-api:latest

# Update frontend
echo ""
echo "Step 3: Updating Frontend UI..."
az containerapp update \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --image $ACR_LOGIN_SERVER/ae-infinity-ui:latest

echo ""
echo "========================================="
echo "✅ Deployment Updated Successfully!"
echo "========================================="
echo ""
echo "🌐 URLs:"
echo "  Frontend: https://$WEB_FQDN"
echo "  Backend: https://$API_FQDN"
echo ""
echo "Monitor deployment:"
echo "  az containerapp revision list -n $API_APP_NAME -g $RESOURCE_GROUP"
echo ""

