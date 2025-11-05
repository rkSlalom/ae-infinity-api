# User Profile Management API

## Overview

This document describes the User Profile Management endpoints for the AE Infinity API (Feature 002).

**Base URL**: `http://localhost:5233/api`

**Authentication**: All endpoints require JWT Bearer token authentication.

---

## Endpoints

### 1. Get Current User Profile

Retrieves the authenticated user's complete profile information.

**Endpoint**: `GET /users/me`

**Authentication**: Required (JWT Bearer token)

**Request Headers**:
```http
Authorization: Bearer {jwt_token}
Content-Type: application/json
```

**Response**: `200 OK`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@example.com",
  "displayName": "John Doe",
  "avatarUrl": "https://example.com/avatars/john.jpg",
  "isEmailVerified": true,
  "lastLoginAt": "2025-11-05T10:30:00Z",
  "createdAt": "2025-01-01T10:00:00Z"
}
```

**Error Responses**:
- `401 Unauthorized`: Invalid or missing JWT token
- `404 Not Found`: User not found in database

---

### 2. Update User Profile

Updates the authenticated user's profile information (display name and/or avatar URL).

**Endpoint**: `PATCH /users/me`

**Authentication**: Required (JWT Bearer token)

**Request Headers**:
```http
Authorization: Bearer {jwt_token}
Content-Type: application/json
```

**Request Body**:
```json
{
  "displayName": "John Doe Updated",
  "avatarUrl": "https://example.com/avatars/new-avatar.jpg"
}
```

**Field Validation**:
- `displayName` (required):
  - Minimum length: 2 characters
  - Maximum length: 100 characters
  - Supports Unicode and emojis
- `avatarUrl` (optional):
  - Must be a valid HTTP/HTTPS URL
  - Maximum length: 500 characters
  - Can be `null` or empty string to clear avatar

**Response**: `200 OK`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@example.com",
  "displayName": "John Doe Updated",
  "avatarUrl": "https://example.com/avatars/new-avatar.jpg",
  "isEmailVerified": true,
  "lastLoginAt": "2025-11-05T10:30:00Z",
  "createdAt": "2025-01-01T10:00:00Z"
}
```

**Error Responses**:
- `400 Bad Request`: Validation errors
  ```json
  {
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "errors": {
      "DisplayName": ["Display name must be at least 2 characters"],
      "AvatarUrl": ["Avatar URL must be a valid URL or empty"]
    }
  }
  ```
- `401 Unauthorized`: Invalid or missing JWT token
- `404 Not Found`: User not found in database

**Real-time Updates**:
- Upon successful update, a `ProfileUpdated` SignalR event is broadcast to all connected clients
- Event payload: `{ userId, displayName, avatarUrl, updatedAt }`

---

### 3. Get User Statistics

Retrieves activity statistics for the authenticated user.

**Endpoint**: `GET /users/me/stats`

**Authentication**: Required (JWT Bearer token)

**Request Headers**:
```http
Authorization: Bearer {jwt_token}
Content-Type: application/json
```

**Response**: `200 OK`

```json
{
  "totalListsOwned": 5,
  "totalListsShared": 12,
  "totalItemsCreated": 87,
  "totalItemsPurchased": 54,
  "totalActiveCollaborations": 8,
  "lastActivityAt": "2025-11-05T14:22:00Z"
}
```

**Field Descriptions**:
- `totalListsOwned`: Count of shopping lists where user is the owner
- `totalListsShared`: Count of lists shared with user (as collaborator)
- `totalItemsCreated`: Total count of items created by user across all lists
- `totalItemsPurchased`: Total count of items marked as purchased by user
- `totalActiveCollaborations`: Count of active (non-archived) collaborative lists
- `lastActivityAt`: Timestamp of user's most recent activity (UTC), or `null` for new users

**Caching**:
- Statistics are cached for 5 minutes to improve performance
- Cache is invalidated when user performs relevant actions (create list, add item, mark purchased)

**Performance**:
- Queries complete in < 500ms even for users with 100+ lists
- Performance warnings logged if query exceeds 500ms threshold

**Error Responses**:
- `401 Unauthorized`: Invalid or missing JWT token

---

### 4. Get Public User Profile

Retrieves limited public profile information for any user (for viewing collaborators).

**Endpoint**: `GET /users/{userId}`

**Authentication**: Required (JWT Bearer token)

**Request Headers**:
```http
Authorization: Bearer {jwt_token}
Content-Type: application/json
```

**Path Parameters**:
- `userId` (GUID): The ID of the user to retrieve

**Response**: `200 OK`

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "displayName": "John Doe",
  "avatarUrl": "https://example.com/avatars/john.jpg"
}
```

**Privacy**:
- Only returns `displayName` and `avatarUrl`
- Does NOT expose: email, statistics, or other sensitive information

**Error Responses**:
- `401 Unauthorized`: Invalid or missing JWT token
- `404 Not Found`: User not found or deleted

---

## Authorization Model

### User-Owned Resources

- Users can **only update their own profile**
- User ID is extracted from the JWT token claims (`ClaimTypes.NameIdentifier`)
- No endpoint allows updating another user's profile

### Security Measures

1. **JWT Validation**: All endpoints require valid JWT Bearer token
2. **User ID from Token**: User ID is extracted from authenticated JWT claims (cannot be spoofed)
3. **No Admin Overrides**: No endpoints allow administrators to edit user profiles (users have full autonomy)
4. **Input Validation**: FluentValidation ensures all inputs meet security constraints
5. **URL Validation**: Avatar URLs must be valid HTTP/HTTPS URIs (prevents XSS)

---

## SignalR Real-time Events

### ProfileUpdated Event

Broadcast when any user updates their profile.

**Event Name**: `ProfileUpdated`

**Payload**:
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "displayName": "John Doe Updated",
  "avatarUrl": "https://example.com/avatars/new-avatar.jpg",
  "updatedAt": "2025-11-05T15:30:00Z"
}
```

**Broadcast Scope**: All connected clients

**Use Cases**:
- Update user avatar/name in Header component
- Refresh collaborator lists in shared lists
- Update activity feeds showing user actions

---

## Examples

### Example 1: Update Display Name Only

**Request**:
```http
PATCH /api/users/me
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "displayName": "Jane Smith 🎉",
  "avatarUrl": null
}
```

**Response**: `200 OK` (returns updated user profile)

---

### Example 2: Clear Avatar

**Request**:
```http
PATCH /api/users/me
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "displayName": "John Doe",
  "avatarUrl": null
}
```

**Response**: `200 OK` (avatar set to `null`, will display initials in UI)

---

### Example 3: Get Statistics

**Request**:
```http
GET /api/users/me/stats
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response**: `200 OK`
```json
{
  "totalListsOwned": 3,
  "totalListsShared": 5,
  "totalItemsCreated": 42,
  "totalItemsPurchased": 28,
  "totalActiveCollaborations": 4,
  "lastActivityAt": "2025-11-05T14:00:00Z"
}
```

---

## Logging & Monitoring

### Profile Update Logging

When a profile is updated, the following structured log is emitted:

```
[Information] Profile updated successfully for User {UserId}. 
DisplayName changed: {true/false} ('{OldName}' -> '{NewName}'), 
AvatarUrl changed: {true/false} ('{OldUrl}' -> '{NewUrl}')
```

### Statistics Performance Logging

When statistics are calculated, performance is tracked:

```
[Information] Statistics calculated for User {UserId} in {ElapsedMs}ms
```

If query exceeds 500ms threshold:

```
[Warning] PERFORMANCE: Statistics query for User {UserId} took {ElapsedMs}ms (threshold: 500ms). 
Lists: {ListsOwned}, Items: {ItemsCreated}. Consider adding database indexes.
```

---

## Rate Limiting

**Recommendations**:
- Profile updates: Maximum 10 requests per minute per user
- Statistics queries: Cached for 5 minutes (reduces database load)

---

## Related Documentation

- [Specification](../../ae-infinity-context/specs/002-user-profile-management/spec.md)
- [Implementation Plan](../../ae-infinity-context/specs/002-user-profile-management/plan.md)
- [Data Model](../../ae-infinity-context/specs/002-user-profile-management/data-model.md)
- [API Contracts](../../ae-infinity-context/specs/002-user-profile-management/contracts/)

---

**Last Updated**: 2025-11-05  
**Feature Version**: 002-user-profile-management  
**API Version**: v1

