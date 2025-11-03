# AE Infinity API - Complete Endpoint List

## Overview

This document lists all API endpoints required for the AE Infinity collaborative shopping list application. Endpoints are organized by resource and follow RESTful conventions.

**Base URL:** `/api/v1`

**Authentication:** All endpoints except `/auth/login` require JWT Bearer token authentication.

---

## 1. Authentication & Authorization

### User Authentication (Prototype)
- `POST /auth/login` - Authenticate and receive JWT token
- `POST /auth/logout` - Invalidate current session

---

## 2. Users

### User Profile Management
- `GET /users/{id}` - Get user profile by ID (public info only)
- `PUT /users/me` - Update own profile
- `DELETE /users/me` - Soft delete own account

### User Search (for collaboration)
- `GET /users/search?q={email}` - Search users by email for list sharing

---

## 3. Shopping Lists

### List CRUD Operations
- `GET /lists` - Get all lists for authenticated user (owned + shared)
- `POST /lists` - Create new shopping list
- `GET /lists/{id}` - Get list details with collaborators
- `PUT /lists/{id}` - Update list name and description
- `DELETE /lists/{id}` - Soft delete list (Owner only)

### List Archival
- `PATCH /lists/{id}/archive` - Archive a list
- `PATCH /lists/{id}/unarchive` - Unarchive a list

### List Ownership
- `POST /lists/{id}/transfer-ownership` - Transfer ownership to another user

### List Statistics
- `GET /lists/{id}/stats` - Get list statistics (item count, purchased count, etc.)
- `GET /lists/{id}/activity` - Get recent activity/history for the list

---

## 4. List Collaboration (Sharing & Permissions)

### Invite & Share
- `POST /lists/{listId}/share` - Invite user to list via email
- `GET /lists/{listId}/invitations` - Get pending invitations for list
- `DELETE /lists/{listId}/invitations/{invitationId}` - Revoke/cancel invitation

### Accept Invitations
- `GET /invitations` - Get all invitations for current user
- `POST /invitations/{invitationId}/accept` - Accept list invitation
- `POST /invitations/{invitationId}/decline` - Decline list invitation

### Manage Collaborators
- `GET /lists/{listId}/collaborators` - Get all list collaborators with roles
- `PATCH /lists/{listId}/collaborators/{userId}/role` - Change collaborator's role
- `DELETE /lists/{listId}/collaborators/{userId}` - Remove collaborator from list

### Leave List
- `POST /lists/{listId}/leave` - Leave a shared list (collaborator leaves)

---

## 5. Shopping List Items

### Item CRUD Operations
- `GET /lists/{listId}/items` - Get all items in list
- `POST /lists/{listId}/items` - Add new item to list
- `GET /lists/{listId}/items/{itemId}` - Get specific item details
- `PUT /lists/{listId}/items/{itemId}` - Update item details
- `DELETE /lists/{listId}/items/{itemId}` - Soft delete item

### Purchase Management
- `PATCH /lists/{listId}/items/{itemId}/purchase` - Mark item as purchased
- `PATCH /lists/{listId}/items/{itemId}/unpurchase` - Mark item as unpurchased

### Item Filtering (via query parameters)
- `GET /lists/{listId}/items?categoryId={id}` - Filter items by category
- `GET /lists/{listId}/items?isPurchased={bool}` - Filter by purchase status
- `GET /lists/{listId}/items?createdBy={userId}` - Filter by creator

---

## 6. Search

### Global Search
- `GET /search?q={query}` - Search across all lists and items
- `GET /search/lists?q={query}` - Search only in lists
- `GET /search/items?q={query}` - Search only in items

### List-Specific Search
- `GET /lists/{listId}/items/search?q={query}` - Search items within specific list

---

## 7. User Dashboard & Statistics

### Personal Statistics
- `GET /users/me/stats` - Get user's overall statistics
  - Total lists created
  - Total lists shared with user
  - Total items added
  - Recent activity summary

### Purchase History
- `GET /users/me/history` - Get user's purchase history across all lists
- `GET /lists/{listId}/history` - Get purchase history for specific list

---

## 8. Real-time (SignalR Hub)

### Hub Connection
- `WS /hubs/shopping-list` - WebSocket connection for real-time updates

### Client → Server Methods
- `JoinList(listId)` - Subscribe to list updates
- `LeaveList(listId)` - Unsubscribe from list updates
- `UpdatePresence(listId, isActive)` - Update user's viewing status

### Server → Client Events
- `ItemAdded(itemDto)` - Broadcast new item
- `ItemUpdated(itemDto)` - Broadcast item update
- `ItemDeleted(itemId)` - Broadcast item deletion
- `ItemPurchased(itemId, userId, purchasedAt)` - Broadcast purchase
- `ItemUnpurchased(itemId)` - Broadcast unpurchase
- `ListUpdated(listDto)` - Broadcast list changes
- `CollaboratorAdded(userId, listId, role)` - Broadcast new collaborator
- `CollaboratorRemoved(userId, listId)` - Broadcast collaborator removal
- `CollaboratorRoleChanged(userId, listId, newRole)` - Broadcast role change
- `PresenceChanged(userId, listId, isActive)` - Broadcast presence update

---

## 9. Health & Diagnostics

### System Health
- `GET /health` - Health check endpoint
- `GET /health/ready` - Readiness check
- `GET /health/live` - Liveness check

---

## Endpoint Summary by HTTP Method

### GET (Read Operations)
**Users:**
- `/users/{id}`
- `/users/search`

**Lists:**
- `/lists`
- `/lists/{id}`
- `/lists/{id}/stats`
- `/lists/{id}/activity`
- `/lists/{id}/collaborators`
- `/lists/{id}/invitations`

**Invitations:**
- `/invitations`

**Items:**
- `/lists/{listId}/items`
- `/lists/{listId}/items/{itemId}`
- `/lists/{listId}/items/search`

**Search:**
- `/search`
- `/search/lists`
- `/search/items`

**Statistics:**
- `/users/me/stats`
- `/users/me/history`
- `/lists/{listId}/history`

**Health:**
- `/health`
- `/health/ready`
- `/health/live`

### POST (Create Operations)
**Authentication:**
- `/auth/login`
- `/auth/logout`

**Lists:**
- `/lists`
- `/lists/{id}/transfer-ownership`
- `/lists/{listId}/share`
- `/lists/{listId}/leave`

**Invitations:**
- `/invitations/{invitationId}/accept`
- `/invitations/{invitationId}/decline`

**Items:**
- `/lists/{listId}/items`

### PUT (Full Update Operations)
**Users:**
- `/users/me`

**Lists:**
- `/lists/{id}`

**Items:**
- `/lists/{listId}/items/{itemId}`

### PATCH (Partial Update Operations)
**Lists:**
- `/lists/{id}/archive`
- `/lists/{id}/unarchive`
- `/lists/{listId}/collaborators/{userId}/role`

**Items:**
- `/lists/{listId}/items/{itemId}/purchase`
- `/lists/{listId}/items/{itemId}/unpurchase`

### DELETE (Soft Delete Operations)
**Users:**
- `/users/me`

**Lists:**
- `/lists/{id}`
- `/lists/{listId}/invitations/{invitationId}`
- `/lists/{listId}/collaborators/{userId}`

**Items:**
- `/lists/{listId}/items/{itemId}`

---

## Authorization Matrix

| Endpoint | Owner | Editor | Editor-Limited | Viewer | Public |
|----------|-------|--------|----------------|--------|--------|
| **Lists** |
| GET /lists | ✅ | ✅ | ✅ | ✅ | ❌ |
| POST /lists | ✅ | ✅ | ✅ | ✅ | ❌ |
| GET /lists/{id} (includes role info) | ✅ | ✅ | ✅ | ✅ | ❌ |
| PUT /lists/{id} | ✅ | ❌ | ❌ | ❌ | ❌ |
| DELETE /lists/{id} | ✅ | ❌ | ❌ | ❌ | ❌ |
| PATCH /lists/{id}/archive | ✅ | ❌ | ❌ | ❌ | ❌ |
| POST /lists/{id}/share | ✅ | ❌ | ❌ | ❌ | ❌ |
| GET /lists/{id}/collaborators | ✅ | ✅ | ✅ | ✅ | ❌ |
| PATCH collaborator role | ✅ | ❌ | ❌ | ❌ | ❌ |
| DELETE collaborator | ✅ | ❌ | ❌ | ❌ | ❌ |
| **Items** |
| GET items | ✅ | ✅ | ✅ | ✅ | ❌ |
| POST items | ✅ | ✅ | ✅ | ❌ | ❌ |
| PUT items/{id} | ✅ | ✅ | ✅* | ❌ | ❌ |
| DELETE items/{id} | ✅ | ✅ | ✅* | ❌ | ❌ |
| PATCH purchase | ✅ | ✅ | ✅ | ❌ | ❌ |

*Editor-Limited can only edit/delete their own items

---

## Query Parameters

### Pagination (applies to list endpoints)
- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 20, max: 100)

### Sorting
- `sortBy` - Field to sort by (default varies by endpoint)
- `sortOrder` - `asc` or `desc` (default: varies)

### Filtering
- `includeArchived` - Include archived lists (boolean)
- `includeCompleted` - Include purchased items (boolean)
- `categoryId` - Filter by category
- `createdBy` - Filter by creator
- `isPurchased` - Filter by purchase status

### Search
- `q` - Search query string
- `scope` - Search scope: `all`, `lists`, `items`

---

## Response Status Codes

### Success
- `200 OK` - Successful GET, PUT, PATCH
- `201 Created` - Successful POST
- `204 No Content` - Successful DELETE

### Client Errors
- `400 Bad Request` - Invalid input
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `409 Conflict` - Duplicate or conflict
- `422 Unprocessable Entity` - Validation failed

### Server Errors
- `500 Internal Server Error` - Server error
- `503 Service Unavailable` - Service temporarily unavailable

---

## Rate Limiting

- **Authenticated Users:** 100 requests/minute
- **Anonymous Users:** 20 requests/minute
- **Search Endpoints:** 30 requests/minute

Rate limit headers:
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1730649000
```

---

## Implementation Priority (Prototype)

### Phase 1: MVP (Core Functionality)
1. Authentication (login, logout only)
2. List CRUD (create, read, update, delete, archive)
3. Item CRUD (create, read, update, delete)
4. Purchase marking (purchase, unpurchase)
5. Categories embedded in item responses
6. Basic user profile

### Phase 2: Collaboration
1. List sharing via email
2. Invitation workflow (invite, accept, decline)
3. Collaborator management (view, remove, change role)
4. Role permissions enforcement
5. User search for sharing

### Phase 3: Real-time & Search
1. Real-time updates (SignalR)
2. Presence indicators
3. Search functionality (lists and items)
4. Item filtering
5. Statistics and history

---

## Next Steps

1. Review and approve endpoint list
2. Create DTOs for request/response bodies
3. Implement domain entities based on DB schema
4. Create CQRS commands and queries
5. Implement controllers with proper authorization
6. Add FluentValidation for all commands
7. Implement SignalR hub
8. Add comprehensive API documentation (Swagger)

---

**Total Endpoints (Prototype):** ~45 REST endpoints + SignalR real-time methods

This streamlined API focuses on core functionality for rapid prototype development while maintaining a solid foundation for the collaborative shopping list application.

## Notes

- **Categories:** Returned as part of item responses. No separate category management endpoints.
- **Roles:** Role information included in `GET /lists/{id}` response for collaborators.
- **Authentication:** Simplified to login/logout only for prototype. User registration can be done via database seeding or admin tool.
- **Profile:** Basic user profile management without avatar upload functionality.
- **Items:** Standard CRUD operations with purchase tracking and filtering, no bulk operations or reordering.

