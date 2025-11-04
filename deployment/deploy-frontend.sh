#!/bin/bash

# Frontend Deployment Script
# Deploys React app to Azure Static Web Apps

# Load configuration
if [ -f "azure-config.env" ]; then
    source azure-config.env
else
    echo "❌ Error: azure-config.env not found. Run azure-setup.sh first."
    exit 1
fi

echo "========================================="
echo "Deploying Frontend to Azure Static Web Apps"
echo "========================================="
echo ""
echo "Target: $WEB_APP_NAME"
echo "Resource Group: $RESOURCE_GROUP"
echo ""

# Navigate to UI project
cd ../../ae-infinity-ui

# Step 1: Update API endpoint in environment
echo "Step 1: Creating production environment config..."
cat > .env.production << EOF
VITE_API_URL=https://$API_APP_NAME.azurewebsites.net/api
EOF

# Step 2: Install dependencies
echo "Step 2: Installing dependencies..."
npm install

# Step 3: Build the application
echo "Step 3: Building React application..."
npm run build

# Step 4: Get Static Web App deployment token
echo "Step 4: Getting deployment token..."
DEPLOYMENT_TOKEN=$(az staticwebapp secrets list \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --query "properties.apiKey" -o tsv)

# Step 5: Deploy using SWA CLI
echo "Step 5: Deploying to Azure Static Web Apps..."

# Check if SWA CLI is installed
if ! command -v swa &> /dev/null; then
    echo "Installing Azure Static Web Apps CLI..."
    npm install -g @azure/static-web-apps-cli
fi

swa deploy ./dist \
  --deployment-token $DEPLOYMENT_TOKEN \
  --app-location ./dist \
  --env production

echo ""
echo "========================================="
echo "✅ Frontend Deployed Successfully!"
echo "========================================="
echo ""
echo "Frontend URL: https://$WEB_URL"
echo ""
echo "Next steps:"
echo "1. Visit the URL above"
echo "2. Login with test credentials from MOCK_CREDENTIALS.md"
echo "3. Test the application"
echo ""

