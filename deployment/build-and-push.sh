#!/bin/bash

# Build and Push Docker Images Script
# Builds both backend and frontend images and pushes to Azure Container Registry

# Load configuration
if [ -f "azure-config.env" ]; then
    source azure-config.env
else
    echo "❌ Error: azure-config.env not found. Run azure-container-setup.sh first."
    exit 1
fi

echo "========================================="
echo "Building and Pushing Docker Images"
echo "========================================="
echo ""
echo "Container Registry: $ACR_LOGIN_SERVER"
echo ""

# Check Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker Desktop."
    exit 1
fi

# Step 1: Login to Azure Container Registry
echo "Step 1: Logging into Azure Container Registry..."
echo "$ACR_PASSWORD" | docker login $ACR_LOGIN_SERVER -u $ACR_USERNAME --password-stdin

# Step 2: Build Backend API Image
echo "Step 2: Building Backend API Docker Image..."
cd ../
docker build -t $ACR_LOGIN_SERVER/ae-infinity-api:latest -f Dockerfile .
docker tag $ACR_LOGIN_SERVER/ae-infinity-api:latest $ACR_LOGIN_SERVER/ae-infinity-api:$(date +%Y%m%d%H%M%S)

# Step 3: Build Frontend UI Image
echo "Step 3: Building Frontend UI Docker Image..."
cd ../ae-infinity-ui
docker build \
  --build-arg VITE_API_URL=https://${API_APP_NAME}.${LOCATION}.azurecontainerapps.io/api \
  -t $ACR_LOGIN_SERVER/ae-infinity-ui:latest \
  -f Dockerfile .
docker tag $ACR_LOGIN_SERVER/ae-infinity-ui:latest $ACR_LOGIN_SERVER/ae-infinity-ui:$(date +%Y%m%d%H%M%S)

# Step 4: Push Backend Image to ACR
echo "Step 4: Pushing Backend API to Container Registry..."
docker push $ACR_LOGIN_SERVER/ae-infinity-api:latest

# Step 5: Push Frontend Image to ACR
echo "Step 5: Pushing Frontend UI to Container Registry..."
docker push $ACR_LOGIN_SERVER/ae-infinity-ui:latest

echo ""
echo "========================================="
echo "✅ Images Built and Pushed Successfully!"
echo "========================================="
echo ""
echo "Images in Registry:"
echo "  - $ACR_LOGIN_SERVER/ae-infinity-api:latest"
echo "  - $ACR_LOGIN_SERVER/ae-infinity-ui:latest"
echo ""
echo "Next step: Run ./deploy-containers.sh to deploy to Azure"
echo ""

# Return to deployment directory
cd ../ae-infinity-api/deployment

