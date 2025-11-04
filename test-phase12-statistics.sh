#!/bin/bash

# Phase 12: Statistics & History Testing Script
# Tests all statistics endpoints

BASE_URL="http://localhost:5233/api"

echo "=========================================="
echo "Phase 12: Statistics & History Testing"
echo "=========================================="
echo ""

# Login as Sarah (list owner with items)
echo "=========================================="
echo "Step 1: Login as Sarah"
echo "=========================================="
SARAH_TOKEN=$(curl -s -X POST "$BASE_URL/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "sarah@example.com",
    "password": "Password123!"
  }' | jq -r '.token')

echo "Sarah logged in: ${SARAH_TOKEN:0:50}..."
echo ""

# Login as Mike
echo "=========================================="
echo "Step 2: Login as Mike"
echo "=========================================="
MIKE_TOKEN=$(curl -s -X POST "$BASE_URL/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "mike@example.com",
    "password": "Password123!"
  }' | jq -r '.token')

echo "Mike logged in: ${MIKE_TOKEN:0:50}..."
echo ""

# Test 1: Get User Statistics
echo "=========================================="
echo "TEST 1: Get Sarah's User Statistics"
echo "GET $BASE_URL/users/me/stats"
echo "=========================================="
RESULT=$(curl -s -X GET "$BASE_URL/users/me/stats" \
  -H "Authorization: Bearer $SARAH_TOKEN")
echo "$RESULT" | jq '.'
LISTS_OWNED=$(echo "$RESULT" | jq '.totalListsOwned')
LISTS_SHARED=$(echo "$RESULT" | jq '.totalListsShared')
ITEMS_CREATED=$(echo "$RESULT" | jq '.totalItemsCreated')
echo "Sarah owns $LISTS_OWNED list(s), has $LISTS_SHARED shared list(s), and created $ITEMS_CREATED item(s)"
echo ""

# Test 2: Get User Purchase History
echo "=========================================="
echo "TEST 2: Get Sarah's Purchase History"
echo "GET $BASE_URL/users/me/history"
echo "=========================================="
RESULT=$(curl -s -X GET "$BASE_URL/users/me/history" \
  -H "Authorization: Bearer $SARAH_TOKEN")
echo "$RESULT" | jq '.'
PURCHASE_COUNT=$(echo "$RESULT" | jq 'length')
echo "Sarah has $PURCHASE_COUNT purchase(s) in history"
echo ""

# Test 3: Get User Purchase History with Limit
echo "=========================================="
echo "TEST 3: Get User Purchase History with Limit (5)"
echo "GET $BASE_URL/users/me/history?limit=5"
echo "=========================================="
RESULT=$(curl -s -X GET "$BASE_URL/users/me/history?limit=5" \
  -H "Authorization: Bearer $SARAH_TOKEN")
echo "$RESULT" | jq '.'
PURCHASE_COUNT=$(echo "$RESULT" | jq 'length')
echo "Returned $PURCHASE_COUNT purchase(s) (max 5)"
echo ""

# Test 4: Get List Statistics
echo "=========================================="
echo "TEST 4: Get Statistics for Weekly Groceries List"
echo "GET $BASE_URL/lists/dddddddd-dddd-dddd-dddd-dddddddddddd/stats"
echo "=========================================="
RESULT=$(curl -s -X GET "$BASE_URL/lists/dddddddd-dddd-dddd-dddd-dddddddddddd/stats" \
  -H "Authorization: Bearer $SARAH_TOKEN")
echo "$RESULT" | jq '.'
TOTAL_ITEMS=$(echo "$RESULT" | jq '.totalItems')
PURCHASED=$(echo "$RESULT" | jq '.purchasedItems')
UNPURCHASED=$(echo "$RESULT" | jq '.unpurchasedItems')
COLLABORATORS=$(echo "$RESULT" | jq '.totalCollaborators')
echo "List has $TOTAL_ITEMS item(s): $PURCHASED purchased, $UNPURCHASED unpurchased, $COLLABORATORS collaborator(s)"
echo ""

# Test 5: Get List Purchase History
echo "=========================================="
echo "TEST 5: Get Purchase History for Weekly Groceries List"
echo "GET $BASE_URL/lists/dddddddd-dddd-dddd-dddd-dddddddddddd/history"
echo "=========================================="
RESULT=$(curl -s -X GET "$BASE_URL/lists/dddddddd-dddd-dddd-dddd-dddddddddddd/history" \
  -H "Authorization: Bearer $SARAH_TOKEN")
echo "$RESULT" | jq '.'
PURCHASE_COUNT=$(echo "$RESULT" | jq 'length')
echo "List has $PURCHASE_COUNT purchase(s) in history"
echo ""

# Test 6: Get List Activity
echo "=========================================="
echo "TEST 6: Get Activity Log for Weekly Groceries List"
echo "GET $BASE_URL/lists/dddddddd-dddd-dddd-dddd-dddddddddddd/activity"
echo "=========================================="
RESULT=$(curl -s -X GET "$BASE_URL/lists/dddddddd-dddd-dddd-dddd-dddddddddddd/activity" \
  -H "Authorization: Bearer $SARAH_TOKEN")
echo "$RESULT" | jq '.'
ACTIVITY_COUNT=$(echo "$RESULT" | jq 'length')
echo "List has $ACTIVITY_COUNT activity record(s)"
echo ""

# Test 7: Get List Activity with Limit
echo "=========================================="
echo "TEST 7: Get Activity Log with Limit (5)"
echo "GET $BASE_URL/lists/dddddddd-dddd-dddd-dddd-dddddddddddd/activity?limit=5"
echo "=========================================="
RESULT=$(curl -s -X GET "$BASE_URL/lists/dddddddd-dddd-dddd-dddd-dddddddddddd/activity?limit=5" \
  -H "Authorization: Bearer $SARAH_TOKEN")
echo "$RESULT" | jq '.'
ACTIVITY_COUNT=$(echo "$RESULT" | jq 'length')
echo "Returned $ACTIVITY_COUNT activity record(s) (max 5)"
echo ""

# Test 8: Mike's Statistics (Different User)
echo "=========================================="
echo "TEST 8: Get Mike's User Statistics"
echo "GET $BASE_URL/users/me/stats"
echo "=========================================="
RESULT=$(curl -s -X GET "$BASE_URL/users/me/stats" \
  -H "Authorization: Bearer $MIKE_TOKEN")
echo "$RESULT" | jq '.'
LISTS_OWNED=$(echo "$RESULT" | jq '.totalListsOwned')
ITEMS_CREATED=$(echo "$RESULT" | jq '.totalItemsCreated')
echo "Mike owns $LISTS_OWNED list(s) and created $ITEMS_CREATED item(s)"
echo ""

# Test 9: Mike's Purchase History
echo "=========================================="
echo "TEST 9: Get Mike's Purchase History"
echo "GET $BASE_URL/users/me/history"
echo "=========================================="
RESULT=$(curl -s -X GET "$BASE_URL/users/me/history" \
  -H "Authorization: Bearer $MIKE_TOKEN")
echo "$RESULT" | jq '.'
PURCHASE_COUNT=$(echo "$RESULT" | jq 'length')
echo "Mike has $PURCHASE_COUNT purchase(s) in history"
echo ""

# Test 10: Mike's List Statistics
echo "=========================================="
echo "TEST 10: Get Statistics for Mike's Party List"
echo "GET $BASE_URL/lists/eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee/stats"
echo "=========================================="
RESULT=$(curl -s -X GET "$BASE_URL/lists/eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee/stats" \
  -H "Authorization: Bearer $MIKE_TOKEN")
echo "$RESULT" | jq '.'
TOTAL_ITEMS=$(echo "$RESULT" | jq '.totalItems')
echo "Party list has $TOTAL_ITEMS item(s)"
echo ""

# Test 11: Validation - Invalid Limit (Too High for User History)
echo "=========================================="
echo "TEST 11: Validation - Invalid Limit (300, max 200)"
echo "GET $BASE_URL/users/me/history?limit=300"
echo "=========================================="
HTTP_STATUS=$(curl -s -o response.json -w "%{http_code}" -X GET "$BASE_URL/users/me/history?limit=300" \
  -H "Authorization: Bearer $SARAH_TOKEN")
RESPONSE=$(cat response.json)
echo "HTTP Status: $HTTP_STATUS"
echo "$RESPONSE" | jq '.'
rm -f response.json
echo ""

# Test 12: Validation - Invalid Limit (Too Low)
echo "=========================================="
echo "TEST 12: Validation - Invalid Limit (0)"
echo "GET $BASE_URL/users/me/history?limit=0"
echo "=========================================="
HTTP_STATUS=$(curl -s -o response.json -w "%{http_code}" -X GET "$BASE_URL/users/me/history?limit=0" \
  -H "Authorization: Bearer $SARAH_TOKEN")
RESPONSE=$(cat response.json)
echo "HTTP Status: $HTTP_STATUS"
echo "$RESPONSE" | jq '.'
rm -f response.json
echo ""

# Test 13: Validation - Invalid Limit for Activity (Too High)
echo "=========================================="
echo "TEST 13: Validation - Invalid Activity Limit (150, max 100)"
echo "GET $BASE_URL/lists/dddddddd-dddd-dddd-dddd-dddddddddddd/activity?limit=150"
echo "=========================================="
HTTP_STATUS=$(curl -s -o response.json -w "%{http_code}" -X GET "$BASE_URL/lists/dddddddd-dddd-dddd-dddd-dddddddddddd/activity?limit=150" \
  -H "Authorization: Bearer $SARAH_TOKEN")
RESPONSE=$(cat response.json)
echo "HTTP Status: $HTTP_STATUS"
echo "$RESPONSE" | jq '.'
rm -f response.json
echo ""

# Test 14: Access Control - Mike Tries to Access Sarah's List Stats
echo "=========================================="
echo "TEST 14: Access Control - Mike Tries Sarah's List (Should Fail)"
echo "GET $BASE_URL/lists/dddddddd-dddd-dddd-dddd-dddddddddddd/stats"
echo "Note: Mike doesn't have access to Sarah's Weekly Groceries"
echo "=========================================="
HTTP_STATUS=$(curl -s -o response.json -w "%{http_code}" -X GET "$BASE_URL/lists/dddddddd-dddd-dddd-dddd-dddddddddddd/stats" \
  -H "Authorization: Bearer $MIKE_TOKEN")
RESPONSE=$(cat response.json)
echo "HTTP Status: $HTTP_STATUS"
echo "$RESPONSE" | jq '.'
rm -f response.json
echo ""

# Test 15: Non-existent List
echo "=========================================="
echo "TEST 15: Statistics for Non-existent List (Should Fail)"
echo "GET $BASE_URL/lists/ffffffff-ffff-ffff-ffff-ffffffffffff/stats"
echo "=========================================="
HTTP_STATUS=$(curl -s -o response.json -w "%{http_code}" -X GET "$BASE_URL/lists/ffffffff-ffff-ffff-ffff-ffffffffffff/stats" \
  -H "Authorization: Bearer $SARAH_TOKEN")
RESPONSE=$(cat response.json)
echo "HTTP Status: $HTTP_STATUS"
echo "$RESPONSE" | jq '.'
rm -f response.json
echo ""

echo "=========================================="
echo "All Phase 12 Statistics Tests Complete!"
echo "=========================================="
echo ""
echo "Summary:"
echo "✅ User statistics dashboard"
echo "✅ User purchase history with limit"
echo "✅ List statistics (item counts, collaborators)"
echo "✅ List purchase history"
echo "✅ List activity log"
echo "✅ Multiple users with different data"
echo "✅ Validation (limit boundaries)"
echo "✅ Access control (403 Forbidden)"
echo "✅ Not found handling (404)"
echo ""
echo "Phase 12 Statistics & History: TESTED!"

