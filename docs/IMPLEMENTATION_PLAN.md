# AE Infinity API - Implementation Plan

## Overview

This document outlines a phased implementation approach for the AE Infinity collaborative shopping list API. Each phase is designed to be small, testable, and incrementally adds functionality to the system.

**Architecture:** Clean Architecture with CQRS pattern  
**Database:** SQLite In-Memory with Soft Delete  
**Authentication:** JWT Bearer Token

---

## Phase 1: Domain Layer - Core Entities

**Goal:** Create all domain entities with proper base classes and audit fields.

### Tasks:
1. ✅ Review existing `BaseEntity` and `BaseAuditableEntity` (already created)
2. Create `User` entity
3. Create `Role` entity
4. Create `List` entity (shopping list)
5. Create `UserToList` entity (junction table for collaboration)
6. Create `Category` entity
7. Create `ListItem` entity
8. Create domain exceptions:
   - `NotFoundException`
   - `UnauthorizedException`
   - `ValidationException`
   - `ForbiddenException`

### Validation:
- All entities must inherit from appropriate base class
- All audit columns present (created_by, created_at, modified_by, modified_at, deleted_by, deleted_at, is_deleted)
- Proper navigation properties defined
- No business logic in entities (just data structure)

### Deliverables:
- [x] 6 entity classes in `Domain/Entities/`
- [x] 4 exception classes in `Domain/Exceptions/`
- [x] All entities compile without errors
- [x] No dependencies on other layers

**Estimated Time:** 2-3 hours  
**Status:** ✅ **COMPLETED**

---

## Phase 2: Infrastructure - Database Configuration

**Goal:** Set up Entity Framework Core configurations and database context.

### Tasks:
1. Update `IApplicationDbContext` interface to include all DbSets
2. Create EF Core entity configurations (IEntityTypeConfiguration):
   - `UserConfiguration`
   - `RoleConfiguration`
   - `ListConfiguration`
   - `UserToListConfiguration`
   - `CategoryConfiguration`
   - `ListItemConfiguration`
3. Update `ApplicationDbContext`:
   - Add all DbSets
   - Register all configurations
   - Configure soft delete query filter
4. Fix SQLite in-memory connection (already done ✅)
5. Test database context creation

### Entity Configurations Should Include:
- Primary keys
- Foreign keys with relationships
- Column types and constraints
- Indexes for performance
- Soft delete query filters: `.HasQueryFilter(e => !e.IsDeleted)`
- Check constraints from DB schema

### Validation:
- Database context builds successfully
- All relationships properly configured
- Soft delete filter applied to all entities
- Can create in-memory database instance

### Deliverables:
- [x] 6 entity configuration classes in `Infrastructure/Persistence/Configurations/`
- [x] Updated `IApplicationDbContext` with all DbSets
- [x] Updated `ApplicationDbContext` with configurations
- [x] Global soft delete query filter
- [x] Build succeeds

**Estimated Time:** 3-4 hours  
**Status:** ✅ **COMPLETED**

---

## Phase 3: Infrastructure - Database Seeder

**Goal:** Create comprehensive seed data for development and testing.

### Tasks:
1. Create `DbSeeder` class with methods:
   - `SeedRoles()` - 4 default roles (Owner, Editor, Editor-Limited, Viewer)
   - `SeedCategories()` - 10 default categories with icons/colors
   - `SeedUsers()` - 3-5 test users with hashed passwords
   - `SeedLists()` - 2-3 sample shopping lists
   - `SeedListItems()` - 15-20 sample items across lists
   - `SeedCollaborations()` - User-to-list relationships with different roles
2. Update `DbInitializer` to call seeder
3. Implement password hashing utility
4. Use consistent GUIDs for seed data (for testing)

### Seed Data Details:

**Roles:**
- Owner (all permissions true)
- Editor (item management only)
- Editor-Limited (add items, edit own items only)
- Viewer (all permissions false)

**Categories:**
- Produce, Dairy, Meat, Bakery, Beverages, Snacks, Frozen, Household, Personal Care, Other
- Each with emoji icon and color code

**Users:**
- User 1: sarah@example.com (Owner)
- User 2: mike@example.com (Editor)
- User 3: emma@example.com (Viewer)
- Password: "Password123!" (hashed)

**Lists:**
- "Weekly Groceries" (owned by Sarah)
- "Party Supplies" (owned by Mike)

**Collaborations:**
- Sarah owns "Weekly Groceries", Mike is Editor, Emma is Viewer
- Mike owns "Party Supplies", Sarah is Editor

### Validation:
- Database initializes with seed data
- All relationships properly seeded
- No foreign key violations
- Passwords properly hashed
- Can query seeded data

### Deliverables:
- [x] `DbSeeder` class in `Infrastructure/Persistence/`
- [x] Password hashing utility
- [x] Updated `DbInitializer` to seed data
- [x] Test data initialization on app startup
- [x] Verify data in database

**Estimated Time:** 3-4 hours  
**Status:** ✅ **COMPLETED**

---

## Phase 4: Application Layer - CQRS Infrastructure

**Goal:** Set up MediatR, FluentValidation, and AutoMapper infrastructure.

### Tasks:
1. ✅ Verify MediatR registered (already done)
2. ✅ Verify FluentValidation registered (already done)
3. ✅ Verify AutoMapper registered (already done)
4. Create base classes/interfaces:
   - `ICommand<TResponse>` interface (optional, for clarity)
   - `IQuery<TResponse>` interface (optional, for clarity)
5. Update `MappingProfile` with all entity-to-DTO mappings
6. Test MediatR pipeline with a simple command

### Validation:
- All CQRS infrastructure registered
- Validation behavior works
- AutoMapper can map entities to DTOs
- Pipeline executes successfully

### Deliverables:
- [x] Updated `MappingProfile` with basic mappings
- [x] Infrastructure tested and verified
- [x] No compilation errors

**Estimated Time:** 1-2 hours  
**Status:** ✅ **COMPLETED**

---

## Phase 5: Authentication - Login & JWT

**Goal:** Implement user authentication with JWT tokens.

### Feature: User Login

### Tasks:

**Application Layer:**
1. Create DTOs:
   - `LoginRequest` (email, password)
   - `LoginResponse` (user info, JWT token)
   - `UserDto` (id, email, displayName, avatarUrl)
2. Create Command:
   - `LoginCommand` : IRequest<LoginResponse>
   - `LoginCommandHandler`
   - `LoginCommandValidator` (email required, password required)
3. Create services interface:
   - `IJwtTokenService` (GenerateToken, ValidateToken)
   - `IPasswordHasher` (HashPassword, VerifyPassword)

**Infrastructure Layer:**
1. Implement `JwtTokenService`:
   - Generate JWT with user claims
   - Configure token expiration (24 hours)
2. Implement `PasswordHasher`:
   - Use BCrypt or Argon2
3. Register services in DI

**API Layer:**
1. Create `AuthController`:
   - `POST /api/auth/login`
2. Configure JWT authentication in `Program.cs`:
   - Add Authentication middleware
   - Add JWT Bearer scheme
   - Configure token validation parameters
3. Test login endpoint

### Validation:
- Can login with valid credentials
- Returns JWT token
- Invalid credentials return 401
- Token includes correct claims
- Token can be validated

### Deliverables:
- [x] Login command, handler, validator
- [x] JWT token service
- [x] Password hasher service
- [x] AuthController with login endpoint
- [x] JWT authentication configured
- [x] Can authenticate successfully
- [x] Test with seeded users

**Estimated Time:** 4-5 hours  
**Status:** ✅ **COMPLETED**

---

## Phase 6: Authentication - Logout & Current User

**Goal:** Complete authentication endpoints.

### Features:
- User Logout
- Get Current User Info

### Tasks:

**Logout:**
1. Create `LogoutCommand` and handler
2. Implement token blacklist (optional for prototype, can be simple)
3. Add `POST /api/auth/logout` endpoint

**Current User:**
1. Create `GetCurrentUserQuery` and handler
2. Create endpoint `GET /api/users/me`
3. Test with authenticated requests

### Validation:
- Logout invalidates session
- Can get current user info with valid token
- Unauthorized without token

### Deliverables:
- [ ] Logout endpoint
- [ ] Current user endpoint
- [ ] Both endpoints tested
- [ ] Authentication working end-to-end

**Estimated Time:** 2-3 hours

---

## Phase 7: User Management - Profile Operations

**Goal:** Implement user profile management.

### Features:
- Get User by ID
- Update Own Profile
- Delete Own Account
- Search Users by Email

### Tasks:

**Application Layer:**
1. Create Queries:
   - `GetUserByIdQuery` and handler
   - `SearchUsersQuery` (by email) and handler
2. Create Commands:
   - `UpdateUserProfileCommand`, handler, validator
   - `DeleteUserAccountCommand` and handler (soft delete)
3. Create DTOs as needed

**API Layer:**
1. Create `UsersController`:
   - `GET /api/users/{id}`
   - `PUT /api/users/me`
   - `DELETE /api/users/me`
   - `GET /api/users/search?q={email}`
2. Add authorization checks
3. Test all endpoints

### Authorization:
- Anyone can get user by ID (public info only)
- Users can only update their own profile
- Users can only delete their own account
- Authenticated users can search for users (for sharing)

### Validation:
- Can view user profiles
- Can update own profile
- Can soft delete account
- Search works with email query
- Authorization enforced

### Deliverables:
- [ ] 2 queries, 2 commands with handlers and validators
- [ ] UsersController with 4 endpoints
- [ ] All endpoints tested
- [ ] Authorization working

**Estimated Time:** 4-5 hours

---

## Phase 8: Shopping Lists - CRUD Operations

**Goal:** Implement core shopping list management.

### Features:
- Get All Lists (for current user)
- Create List
- Get List by ID
- Update List
- Delete List
- Archive/Unarchive List

### Tasks:

**Application Layer:**
1. Create Queries:
   - `GetListsQuery` and handler (filter by user, include shared)
   - `GetListByIdQuery` and handler (with collaborators and stats)
2. Create Commands:
   - `CreateListCommand`, handler, validator
   - `UpdateListCommand`, handler, validator
   - `DeleteListCommand` and handler (soft delete)
   - `ArchiveListCommand` and handler
   - `UnarchiveListCommand` and handler
3. Create DTOs:
   - `ListDto` (full details)
   - `ListSummaryDto` (for list view)
   - `CollaboratorDto`

**Authorization Helper:**
1. Create `IListPermissionService`:
   - `CanUserAccessList(userId, listId)`
   - `CanUserEditList(userId, listId)`
   - `CanUserDeleteList(userId, listId)` (owner only)
   - `GetUserRoleForList(userId, listId)`

**API Layer:**
1. Create `ListsController`:
   - `GET /api/lists`
   - `POST /api/lists`
   - `GET /api/lists/{id}`
   - `PUT /api/lists/{id}`
   - `DELETE /api/lists/{id}`
   - `PATCH /api/lists/{id}/archive`
   - `PATCH /api/lists/{id}/unarchive`
2. Implement authorization checks using permission service
3. Test all endpoints

### Business Rules:
- User automatically becomes Owner when creating list
- Owner cannot be removed from list
- Only Owner can delete list
- Only Owner can archive list
- Soft delete cascades to items (handled by query filter)

### Validation:
- Can create lists
- Can view own and shared lists
- Can update list details (owner only)
- Can delete list (owner only)
- Can archive/unarchive list
- Authorization properly enforced
- List includes collaborator information

### Deliverables:
- [ ] 2 queries, 5 commands with handlers and validators
- [ ] List permission service
- [ ] ListsController with 7 endpoints
- [ ] All endpoints tested
- [ ] Authorization working

**Estimated Time:** 6-8 hours

---

## Phase 9: List Items - CRUD Operations

**Goal:** Implement shopping list item management.

### Features:
- Get All Items in List
- Create Item
- Get Item by ID
- Update Item
- Delete Item
- Mark as Purchased/Unpurchased

### Tasks:

**Application Layer:**
1. Create Queries:
   - `GetListItemsQuery` and handler (with filtering)
   - `GetListItemByIdQuery` and handler
2. Create Commands:
   - `CreateListItemCommand`, handler, validator
   - `UpdateListItemCommand`, handler, validator
   - `DeleteListItemCommand` and handler (soft delete)
   - `MarkItemPurchasedCommand` and handler
   - `MarkItemUnpurchasedCommand` and handler
3. Create DTOs:
   - `ListItemDto` (with category, creator, purchase info)
   - `CategoryDto`
   - Include category info in responses

**Authorization:**
- Must have access to list to view items
- Must be Editor or Owner to create/edit/delete items
- Editor-Limited can only edit own items
- Viewers cannot modify items

### Tasks - Continued:

**API Layer:**
1. Create `ListItemsController`:
   - `GET /api/lists/{listId}/items` (with query filters)
   - `POST /api/lists/{listId}/items`
   - `GET /api/lists/{listId}/items/{itemId}`
   - `PUT /api/lists/{listId}/items/{itemId}`
   - `DELETE /api/lists/{listId}/items/{itemId}`
   - `PATCH /api/lists/{listId}/items/{itemId}/purchase`
   - `PATCH /api/lists/{listId}/items/{itemId}/unpurchase`
2. Implement role-based authorization
3. Add query parameter filtering:
   - `categoryId` - Filter by category
   - `isPurchased` - Filter by purchase status
   - `createdBy` - Filter by creator
4. Test all endpoints

### Business Rules:
- Items automatically get next position when created
- Marking purchased sets purchasedAt, purchasedBy
- Unmarking purchased clears those fields
- Editor-Limited can only edit/delete items they created
- Category must exist (from seeded categories)

### Validation:
- Can create items in list
- Can view items with filtering
- Can update item details
- Can delete items
- Can mark purchased/unpurchased
- Authorization enforced (especially Editor-Limited)
- Categories included in responses

### Deliverables:
- [ ] 2 queries, 5 commands with handlers and validators
- [ ] ListItemsController with 7 endpoints
- [ ] Role-based authorization implemented
- [ ] Query filtering working
- [ ] All endpoints tested

**Estimated Time:** 6-8 hours

---

## Phase 10: List Collaboration - Sharing & Invitations

**Goal:** Implement list sharing and collaboration features.

### Features:
- Share List with User (Send Invitation)
- Get Pending Invitations
- Accept Invitation
- Decline Invitation
- View List Collaborators
- Remove Collaborator
- Change Collaborator Role
- Leave List

### Tasks:

**Application Layer:**
1. Create Commands:
   - `ShareListCommand` (invite user by email), handler, validator
   - `AcceptInvitationCommand` and handler
   - `DeclineInvitationCommand` and handler
   - `RemoveCollaboratorCommand` and handler (owner only)
   - `ChangeCollaboratorRoleCommand`, handler, validator (owner only)
   - `LeaveListCommand` and handler (cannot be owner)
2. Create Queries:
   - `GetUserInvitationsQuery` and handler (for current user)
   - `GetListInvitationsQuery` and handler (pending for a list)
   - `GetListCollaboratorsQuery` and handler (with role info)
3. Create DTOs:
   - `InvitationDto`
   - `CollaboratorDto` (includes role information)
   - `ShareListRequest`

**Business Rules:**
- Only Owner can share list
- Cannot invite user already on list
- User must exist in system to be invited
- Owner cannot be removed
- Owner cannot leave (must transfer ownership first)
- Accepting invitation sets is_pending=false, accepted_at=now

**API Layer:**
1. Update `ListsController` or create `CollaborationController`:
   - `POST /api/lists/{listId}/share`
   - `GET /api/lists/{listId}/collaborators`
   - `DELETE /api/lists/{listId}/collaborators/{userId}`
   - `PATCH /api/lists/{listId}/collaborators/{userId}/role`
   - `POST /api/lists/{listId}/leave`
2. Create `InvitationsController`:
   - `GET /api/invitations` (user's invitations)
   - `GET /api/lists/{listId}/invitations` (list's pending invitations)
   - `POST /api/invitations/{invitationId}/accept`
   - `POST /api/invitations/{invitationId}/decline`
   - `DELETE /api/lists/{listId}/invitations/{invitationId}` (revoke)
3. Implement authorization checks
4. Test all sharing workflows

### Validation:
- Can share list with user by email
- User receives invitation
- Can accept invitation and gain access
- Can decline invitation
- Owner can manage collaborators
- Owner can change roles
- Can remove collaborators
- Non-owners can leave list
- Cannot invite non-existent users
- Cannot invite already-invited users

### Deliverables:
- [ ] 7 commands, 3 queries with handlers and validators
- [ ] Collaboration endpoints (8 total)
- [ ] Authorization checks for owner-only operations
- [ ] All workflows tested
- [ ] Invitation system working end-to-end

**Estimated Time:** 8-10 hours

---

## Phase 11: Search Functionality

**Goal:** Implement search across lists and items.

### Features:
- Global Search (lists + items)
- Search Lists Only
- Search Items Only
- Search Items in Specific List

### Tasks:

**Application Layer:**
1. Create Queries:
   - `SearchGlobalQuery` and handler (searches both)
   - `SearchListsQuery` and handler
   - `SearchItemsQuery` and handler
   - `SearchListItemsQuery` and handler (within specific list)
2. Implement search logic:
   - Search in name, description fields
   - Case-insensitive
   - Partial match (LIKE %query%)
   - Only search accessible lists for user
3. Create DTOs:
   - `SearchResultDto` (lists + items)
   - `ListSearchResultDto`
   - `ItemSearchResultDto`

**API Layer:**
1. Create `SearchController`:
   - `GET /api/search?q={query}`
   - `GET /api/search/lists?q={query}`
   - `GET /api/search/items?q={query}`
2. Add to `ListItemsController`:
   - `GET /api/lists/{listId}/items/search?q={query}`
3. Add query validation (minimum 2 characters)
4. Implement pagination for results
5. Test search functionality

### Validation:
- Can search across all accessible content
- Results only include accessible lists
- Search works with partial matches
- Case-insensitive search
- Pagination works
- Minimum query length enforced

### Deliverables:
- [ ] 4 queries with handlers
- [ ] SearchController with 3 endpoints
- [ ] List-specific search endpoint
- [ ] Pagination implemented
- [ ] All search scenarios tested

**Estimated Time:** 4-5 hours

---

## Phase 12: Statistics & History

**Goal:** Implement user statistics and purchase history.

### Features:
- User Statistics Dashboard
- Purchase History (User)
- Purchase History (List)
- List Statistics

### Tasks:

**Application Layer:**
1. Create Queries:
   - `GetUserStatsQuery` and handler
   - `GetUserPurchaseHistoryQuery` and handler
   - `GetListPurchaseHistoryQuery` and handler
   - `GetListStatsQuery` and handler
2. Create DTOs:
   - `UserStatsDto` (list counts, item counts, recent activity)
   - `ListStatsDto` (item count, purchased count, collaborator count)
   - `PurchaseHistoryDto`
   - `ListActivityDto`
3. Implement aggregation queries:
   - Count lists owned
   - Count lists shared with user
   - Count items added by user
   - Recent purchases

**API Layer:**
1. Create `StatsController`:
   - `GET /api/users/me/stats`
   - `GET /api/users/me/history`
2. Add to `ListsController`:
   - `GET /api/lists/{id}/stats`
   - `GET /api/lists/{id}/history`
   - `GET /api/lists/{id}/activity`
3. Test all statistics endpoints

### Validation:
- Statistics accurately calculated
- History includes purchase details
- Only show accessible data
- Activity log includes recent changes
- Performance acceptable

### Deliverables:
- [ ] 4 queries with handlers
- [ ] Statistics DTOs
- [ ] StatsController with 2 endpoints
- [ ] List statistics endpoints
- [ ] All endpoints tested

**Estimated Time:** 4-5 hours

---

## Phase 13: Real-time Updates with SignalR

**Goal:** Implement real-time collaboration features.

### Features:
- Real-time item updates
- Real-time purchase notifications
- Real-time collaborator changes
- User presence indicators

### Tasks:

**Infrastructure Layer:**
1. Create `ShoppingListHub` in `Hubs/`:
   - Inherit from `Hub`
   - Implement connection management
2. Implement Hub methods:
   - `JoinList(string listId)` - Subscribe to list updates
   - `LeaveList(string listId)` - Unsubscribe
   - `UpdatePresence(string listId, bool isActive)` - Update viewing status

**Application Layer:**
1. Create `IRealtimeNotificationService`:
   - `NotifyItemAdded(listId, item)`
   - `NotifyItemUpdated(listId, item)`
   - `NotifyItemDeleted(listId, itemId)`
   - `NotifyItemPurchased(listId, itemId, userId, timestamp)`
   - `NotifyListUpdated(listId, list)`
   - `NotifyCollaboratorAdded(listId, userId, role)`
   - `NotifyCollaboratorRemoved(listId, userId)`
   - `NotifyPresenceChanged(listId, userId, isActive)`
2. Implement `RealtimeNotificationService`:
   - Inject `IHubContext<ShoppingListHub>`
   - Send notifications to SignalR clients
3. Integrate notifications into command handlers:
   - Call notification service after successful operations
   - Send to all list members except originator

**API Layer:**
1. Configure SignalR in `Program.cs`:
   - Add SignalR services
   - Map hub endpoint `/hubs/shopping-list`
   - Configure CORS for WebSocket
2. Add JWT authentication to SignalR hub
3. Test real-time updates

### Client Events (Server → Client):
- `ItemAdded(itemDto)`
- `ItemUpdated(itemDto)`
- `ItemDeleted(itemId)`
- `ItemPurchased(itemId, userId, purchasedAt)`
- `ItemUnpurchased(itemId)`
- `ListUpdated(listDto)`
- `CollaboratorAdded(userId, listId, role)`
- `CollaboratorRemoved(userId, listId)`
- `PresenceChanged(userId, listId, isActive)`

### Validation:
- SignalR hub connects successfully
- Real-time notifications sent on operations
- Only list members receive notifications
- Presence updates work
- Multiple clients can connect
- Authentication works with SignalR

### Deliverables:
- [ ] ShoppingListHub implementation
- [ ] IRealtimeNotificationService interface and implementation
- [ ] Integration into command handlers
- [ ] SignalR configured
- [ ] Real-time updates tested
- [ ] Presence system working

**Estimated Time:** 6-8 hours

---

## Phase 14: Additional Endpoints & Polish

**Goal:** Complete remaining endpoints and polish the API.

### Features:
- Transfer List Ownership
- Health Check Endpoints
- API Documentation (Swagger)
- Error Handling Improvements

### Tasks:

**Transfer Ownership:**
1. Create `TransferListOwnershipCommand`, handler, validator
2. Add endpoint `POST /api/lists/{id}/transfer-ownership`
3. Update old owner to Editor role
4. Test ownership transfer

**Health Checks:**
1. Add health check endpoints:
   - `GET /health`
   - `GET /health/ready`
   - `GET /health/live`
2. Configure in `Program.cs`
3. Test health endpoints

**Swagger Documentation:**
1. Add XML comments to all controllers
2. Add ProducesResponseType attributes
3. Document request/response schemas
4. Add authentication scheme to Swagger
5. Test Swagger UI

**Error Handling:**
1. Review `ExceptionHandlingMiddleware`
2. Ensure all exceptions properly caught
3. Return consistent error responses
4. Add validation error details
5. Test error scenarios

**Polish:**
1. Add rate limiting middleware
2. Add logging throughout application
3. Add correlation IDs for request tracking
4. Review and optimize queries
5. Add indexes as needed

### Validation:
- Ownership transfer works correctly
- Health checks respond correctly
- Swagger documentation complete and accurate
- All errors handled gracefully
- Logging comprehensive
- Performance acceptable

### Deliverables:
- [ ] Transfer ownership endpoint
- [ ] Health check endpoints
- [ ] Complete Swagger documentation
- [ ] Improved error handling
- [ ] Rate limiting configured
- [ ] Comprehensive logging
- [ ] All endpoints tested

**Estimated Time:** 5-6 hours

---

## Phase 15: Testing & Validation

**Goal:** Comprehensive testing of all functionality.

### Tasks:

**Integration Testing:**
1. Test complete user workflows:
   - User login → Create list → Add items → Mark purchased
   - User login → Share list → Other user accepts → Collaborate
   - User login → Search → View results
2. Test all authorization scenarios
3. Test error handling
4. Test edge cases

**Manual Testing:**
1. Test with Swagger UI
2. Test with HTTP client
3. Test real-time features with multiple connections
4. Test concurrent operations

**Documentation:**
1. Update README with:
   - Setup instructions
   - API endpoint documentation
   - Authentication guide
   - Example requests
2. Create POSTMAN/Thunder Client collection
3. Document known limitations

**Bug Fixes:**
1. Fix any issues found during testing
2. Optimize slow queries
3. Improve error messages
4. Handle edge cases

### Deliverables:
- [ ] All workflows tested
- [ ] Authorization thoroughly validated
- [ ] Documentation complete
- [ ] API collection created
- [ ] All bugs fixed
- [ ] Production-ready API

**Estimated Time:** 6-8 hours

---

## Summary & Time Estimates

### Total Phases: 15

| Phase | Description | Estimated Time |
|-------|-------------|----------------|
| 1 | Domain Entities | 2-3 hours |
| 2 | Database Configuration | 3-4 hours |
| 3 | Database Seeder | 3-4 hours |
| 4 | CQRS Infrastructure | 1-2 hours |
| 5 | Authentication - Login & JWT | 4-5 hours |
| 6 | Authentication - Logout & Current User | 2-3 hours |
| 7 | User Management | 4-5 hours |
| 8 | Shopping Lists CRUD | 6-8 hours |
| 9 | List Items CRUD | 6-8 hours |
| 10 | List Collaboration | 8-10 hours |
| 11 | Search Functionality | 4-5 hours |
| 12 | Statistics & History | 4-5 hours |
| 13 | Real-time SignalR | 6-8 hours |
| 14 | Additional Endpoints & Polish | 5-6 hours |
| 15 | Testing & Validation | 6-8 hours |

**Total Estimated Time:** 65-84 hours (~2-3 weeks for full-time development)

---

## Development Guidelines

### General Rules:
1. **Complete one phase before moving to next**
2. **Test each phase thoroughly before proceeding**
3. **Commit after each phase completion**
4. **Update this plan with actual progress**
5. **Document any deviations or issues**

### Testing Strategy per Phase:
- Unit tests for complex logic (optional for prototype)
- Manual testing with Swagger/HTTP client
- Verify authorization rules
- Test error scenarios
- Check database state

### Code Review Checklist:
- [ ] Follows Clean Architecture principles
- [ ] CQRS pattern properly implemented
- [ ] Authorization checks in place
- [ ] Validation implemented
- [ ] Soft delete handled correctly
- [ ] Audit fields populated
- [ ] Error handling present
- [ ] No business logic in controllers
- [ ] DTOs used for responses
- [ ] AutoMapper configured
- [ ] Logging added where appropriate

---

## Dependencies Between Phases

```
Phase 1 (Entities) → Phase 2 (DB Config) → Phase 3 (Seeder)
                                              ↓
Phase 4 (CQRS Infrastructure) ← ← ← ← ← ← ← ←
                ↓
Phase 5 (Login) → Phase 6 (Logout/CurrentUser) → Phase 7 (User Management)
                                                        ↓
                          Phase 8 (Lists CRUD) ← ← ← ← 
                                    ↓
                          Phase 9 (List Items)
                                    ↓
                          Phase 10 (Collaboration)
                                    ↓
Phase 11 (Search) ← ← ← ← ← ← ← ← ← ← ← ← ← ← ← ← ← ←
                ↓
Phase 12 (Statistics) → Phase 13 (Real-time) → Phase 14 (Polish) → Phase 15 (Testing)
```

**Critical Path:** Phases 1-3 must be completed first. Phases 5-10 are the core functionality. Phases 11-15 enhance the application.

---

## Next Steps

1. Review and approve this implementation plan
2. Begin Phase 1: Create domain entities
3. Track progress using checkboxes
4. Update plan as needed during development
5. Commit completed phases to version control

**Ready to start implementation!** 🚀

