# AE Infinity - Database Schema

## Overview

This document defines the complete database schema for the AE Infinity collaborative shopping list application. The schema implements **soft delete** functionality across all tables, comprehensive **audit trails**, and supports **role-based access control** for list collaboration.

## General Database Rules

### Primary Keys
- All tables use `id` as the primary key
- Type: `GUID` (UUID)
- Auto-generated on insert

### Audit Columns
Every table includes the following audit columns:

| Column Name | Type | Description |
|------------|------|-------------|
| `created_by` | GUID (FK → users.id) | User who created the record |
| `created_at` | DATETIME | Timestamp of creation (UTC) |
| `modified_by` | GUID (FK → users.id) | User who last modified the record |
| `modified_at` | DATETIME | Timestamp of last modification (UTC) |
| `deleted_by` | GUID (FK → users.id) | User who soft-deleted the record |
| `deleted_at` | DATETIME | Timestamp of soft deletion (UTC) |
| `is_deleted` | BOOLEAN | Soft delete flag (default: false) |

### Soft Delete Implementation
- Records are **never physically deleted** from the database
- DELETE operations set `is_deleted = true`, `deleted_at = CURRENT_TIMESTAMP`, and `deleted_by = current_user_id`
- All queries MUST filter by `WHERE is_deleted = false` unless explicitly querying deleted records
- Deleted records can be restored by setting `is_deleted = false` and clearing `deleted_at` and `deleted_by`

### Indexing Strategy
- Primary keys: Clustered index
- Foreign keys: Non-clustered index
- Audit columns: Index on `created_at`, `is_deleted`
- Frequently queried columns: Additional indexes as specified per table

---

## Table Definitions

## 1. `users`

Stores user account information for authentication and identification.

### Schema

| Column Name | Type | Constraints | Description |
|------------|------|-------------|-------------|
| `id` | GUID | PRIMARY KEY | Unique user identifier |
| `email` | VARCHAR(255) | UNIQUE, NOT NULL | User's email address (lowercase) |
| `email_normalized` | VARCHAR(255) | UNIQUE, NOT NULL | Normalized email for case-insensitive lookup |
| `display_name` | VARCHAR(100) | NOT NULL | User's display name |
| `password_hash` | VARCHAR(255) | NOT NULL | Hashed password (bcrypt/argon2) |
| `avatar_url` | VARCHAR(500) | NULL | URL to user's avatar image |
| `is_email_verified` | BOOLEAN | NOT NULL, DEFAULT false | Email verification status |
| `email_verification_token` | VARCHAR(255) | NULL | Token for email verification |
| `password_reset_token` | VARCHAR(255) | NULL | Token for password reset |
| `password_reset_expires_at` | DATETIME | NULL | Expiration time for reset token |
| `last_login_at` | DATETIME | NULL | Last successful login timestamp |
| `created_by` | GUID | FK → users.id | Self-reference for audit |
| `created_at` | DATETIME | NOT NULL | Record creation timestamp |
| `modified_by` | GUID | FK → users.id | Last modifier |
| `modified_at` | DATETIME | NULL | Last modification timestamp |
| `deleted_by` | GUID | FK → users.id | User who soft-deleted |
| `deleted_at` | DATETIME | NULL | Soft delete timestamp |
| `is_deleted` | BOOLEAN | NOT NULL, DEFAULT false | Soft delete flag |

### Indexes
```sql
CREATE INDEX idx_users_email ON users(email_normalized) WHERE is_deleted = false;
CREATE INDEX idx_users_email_verification_token ON users(email_verification_token);
CREATE INDEX idx_users_password_reset_token ON users(password_reset_token);
CREATE INDEX idx_users_is_deleted ON users(is_deleted);
```

### Constraints
```sql
CHECK (email = LOWER(email))
CHECK (LENGTH(display_name) >= 2)
CHECK (LENGTH(password_hash) >= 8)
```

### Sample Data
```sql
-- System user (for seed data and system operations)
id: 00000000-0000-0000-0000-000000000000
email: system@ae-infinity.local
display_name: System
```

---

## 2. `roles`

Defines permission levels for list collaboration.

### Schema

| Column Name | Type | Constraints | Description |
|------------|------|-------------|-------------|
| `id` | GUID | PRIMARY KEY | Unique role identifier |
| `name` | VARCHAR(50) | UNIQUE, NOT NULL | Role name (Owner, Editor, Viewer) |
| `description` | VARCHAR(500) | NOT NULL | Role description |
| `can_create_items` | BOOLEAN | NOT NULL | Permission to add items |
| `can_edit_items` | BOOLEAN | NOT NULL | Permission to edit items |
| `can_delete_items` | BOOLEAN | NOT NULL | Permission to delete items |
| `can_edit_own_items_only` | BOOLEAN | NOT NULL | Restrict editing to own items |
| `can_mark_purchased` | BOOLEAN | NOT NULL | Permission to mark items as purchased |
| `can_edit_list_details` | BOOLEAN | NOT NULL | Permission to edit list name/description |
| `can_manage_collaborators` | BOOLEAN | NOT NULL | Permission to add/remove collaborators |
| `can_delete_list` | BOOLEAN | NOT NULL | Permission to delete entire list |
| `can_archive_list` | BOOLEAN | NOT NULL | Permission to archive list |
| `priority_order` | INT | NOT NULL | Display order (lower = higher priority) |
| `created_by` | GUID | FK → users.id | Creator user ID |
| `created_at` | DATETIME | NOT NULL | Record creation timestamp |
| `modified_by` | GUID | FK → users.id | Last modifier |
| `modified_at` | DATETIME | NULL | Last modification timestamp |
| `deleted_by` | GUID | FK → users.id | User who soft-deleted |
| `deleted_at` | DATETIME | NULL | Soft delete timestamp |
| `is_deleted` | BOOLEAN | NOT NULL, DEFAULT false | Soft delete flag |

### Indexes
```sql
CREATE UNIQUE INDEX idx_roles_name ON roles(name) WHERE is_deleted = false;
CREATE INDEX idx_roles_is_deleted ON roles(is_deleted);
```

### Predefined Roles

| Role | Permissions |
|------|-------------|
| **Owner** | All permissions enabled |
| **Editor** | Can manage items, cannot manage list or collaborators |
| **Editor-Limited** | Can add items, edit own items only, mark purchased |
| **Viewer** | All permissions disabled (read-only) |

### Sample Data
```sql
-- Owner Role
id: 11111111-1111-1111-1111-111111111111
name: Owner
can_* : ALL TRUE
priority_order: 1

-- Editor Role  
id: 22222222-2222-2222-2222-222222222222
name: Editor
can_create_items: true
can_edit_items: true
can_delete_items: true
can_mark_purchased: true
can_edit_own_items_only: false
priority_order: 2

-- Editor-Limited Role
id: 33333333-3333-3333-3333-333333333333
name: Editor-Limited
can_create_items: true
can_edit_items: true
can_edit_own_items_only: true
can_mark_purchased: true
priority_order: 3

-- Viewer Role
id: 44444444-4444-4444-4444-444444444444
name: Viewer
can_* : ALL FALSE
priority_order: 4
```

---

## 3. `lists`

Stores shopping lists.

### Schema

| Column Name | Type | Constraints | Description |
|------------|------|-------------|-------------|
| `id` | GUID | PRIMARY KEY | Unique list identifier |
| `name` | VARCHAR(200) | NOT NULL | List name |
| `description` | TEXT | NULL | Optional list description |
| `owner_id` | GUID | FK → users.id, NOT NULL | List creator and owner |
| `is_archived` | BOOLEAN | NOT NULL, DEFAULT false | Archive status |
| `archived_at` | DATETIME | NULL | Timestamp when archived |
| `archived_by` | GUID | FK → users.id | User who archived the list |
| `created_by` | GUID | FK → users.id | Creator user ID |
| `created_at` | DATETIME | NOT NULL | Record creation timestamp |
| `modified_by` | GUID | FK → users.id | Last modifier |
| `modified_at` | DATETIME | NULL | Last modification timestamp |
| `deleted_by` | GUID | FK → users.id | User who soft-deleted |
| `deleted_at` | DATETIME | NULL | Soft delete timestamp |
| `is_deleted` | BOOLEAN | NOT NULL, DEFAULT false | Soft delete flag |

### Indexes
```sql
CREATE INDEX idx_lists_owner_id ON lists(owner_id) WHERE is_deleted = false;
CREATE INDEX idx_lists_created_at ON lists(created_at) WHERE is_deleted = false;
CREATE INDEX idx_lists_is_archived ON lists(is_archived) WHERE is_deleted = false;
CREATE INDEX idx_lists_is_deleted ON lists(is_deleted);
CREATE INDEX idx_lists_name ON lists(name) WHERE is_deleted = false;
```

### Constraints
```sql
CHECK (LENGTH(name) >= 1 AND LENGTH(name) <= 200)
CHECK (owner_id IS NOT NULL)
CHECK (owner_id = created_by) -- Owner must be creator
```

### Business Rules
- Owner cannot be removed from `user_to_list` table
- Deleting a list soft-deletes all related items and collaborator records
- Owner can transfer ownership by updating `owner_id` (audit trail maintained)

---

## 4. `user_to_list`

Junction table managing list collaboration and permissions.

### Schema

| Column Name | Type | Constraints | Description |
|------------|------|-------------|-------------|
| `id` | GUID | PRIMARY KEY | Unique collaborator record ID |
| `list_id` | GUID | FK → lists.id, NOT NULL | Reference to list |
| `user_id` | GUID | FK → users.id, NOT NULL | Reference to user |
| `role_id` | GUID | FK → roles.id, NOT NULL | Permission level |
| `invited_by` | GUID | FK → users.id, NOT NULL | User who sent invitation |
| `invited_at` | DATETIME | NOT NULL | Invitation timestamp |
| `accepted_at` | DATETIME | NULL | When user accepted invitation |
| `is_pending` | BOOLEAN | NOT NULL, DEFAULT true | Invitation status |
| `created_by` | GUID | FK → users.id | Creator user ID |
| `created_at` | DATETIME | NOT NULL | Record creation timestamp |
| `modified_by` | GUID | FK → users.id | Last modifier |
| `modified_at` | DATETIME | NULL | Last modification timestamp |
| `deleted_by` | GUID | FK → users.id | User who soft-deleted |
| `deleted_at` | DATETIME | NULL | Soft delete timestamp |
| `is_deleted` | BOOLEAN | NOT NULL, DEFAULT false | Soft delete flag |

### Indexes
```sql
CREATE INDEX idx_user_to_list_list_id ON user_to_list(list_id) WHERE is_deleted = false;
CREATE INDEX idx_user_to_list_user_id ON user_to_list(user_id) WHERE is_deleted = false;
CREATE INDEX idx_user_to_list_is_pending ON user_to_list(is_pending) WHERE is_deleted = false;
CREATE UNIQUE INDEX idx_user_to_list_unique ON user_to_list(list_id, user_id) WHERE is_deleted = false;
CREATE INDEX idx_user_to_list_is_deleted ON user_to_list(is_deleted);
```

### Constraints
```sql
CHECK (invited_at <= accepted_at OR accepted_at IS NULL)
UNIQUE (list_id, user_id) WHERE is_deleted = false -- User can only have one active role per list
```

### Business Rules
- Owner must always have an active record with Owner role
- Removing owner requires ownership transfer first
- Accepting invitation sets `is_pending = false` and `accepted_at = CURRENT_TIMESTAMP`
- User cannot invite themselves
- Invited user must accept to gain access

---

## 5. `categories`

Product categories for organizing shopping items.

### Schema

| Column Name | Type | Constraints | Description |
|------------|------|-------------|-------------|
| `id` | GUID | PRIMARY KEY | Unique category identifier |
| `name` | VARCHAR(100) | NOT NULL | Category name |
| `icon` | VARCHAR(50) | NULL | Emoji or icon identifier |
| `color` | VARCHAR(7) | NULL | Hex color code (e.g., #FF5733) |
| `is_default` | BOOLEAN | NOT NULL, DEFAULT false | System-defined category |
| `is_custom` | BOOLEAN | NOT NULL, DEFAULT false | User-created category |
| `custom_owner_id` | GUID | FK → users.id | Owner if custom category |
| `sort_order` | INT | NOT NULL, DEFAULT 999 | Display order |
| `created_by` | GUID | FK → users.id | Creator user ID |
| `created_at` | DATETIME | NOT NULL | Record creation timestamp |
| `modified_by` | GUID | FK → users.id | Last modifier |
| `modified_at` | DATETIME | NULL | Last modification timestamp |
| `deleted_by` | GUID | FK → users.id | User who soft-deleted |
| `deleted_at` | DATETIME | NULL | Soft delete timestamp |
| `is_deleted` | BOOLEAN | NOT NULL, DEFAULT false | Soft delete flag |

### Indexes
```sql
CREATE INDEX idx_categories_is_default ON categories(is_default) WHERE is_deleted = false;
CREATE INDEX idx_categories_custom_owner ON categories(custom_owner_id) WHERE is_deleted = false;
CREATE INDEX idx_categories_sort_order ON categories(sort_order) WHERE is_deleted = false;
CREATE INDEX idx_categories_is_deleted ON categories(is_deleted);
```

### Constraints
```sql
CHECK ((is_custom = true AND custom_owner_id IS NOT NULL) OR (is_custom = false AND custom_owner_id IS NULL))
CHECK (is_default = false OR is_custom = false) -- Cannot be both default and custom
CHECK (color IS NULL OR color ~ '^#[0-9A-Fa-f]{6}$') -- Valid hex color
```

### Predefined Default Categories

| ID | Name | Icon | Color | Sort Order |
|----|------|------|-------|------------|
| `aaaaaaaa-1111-...` | Produce | 🍎 | #C8E6C9 | 1 |
| `aaaaaaaa-2222-...` | Dairy | 🥛 | #E3F2FD | 2 |
| `aaaaaaaa-3333-...` | Meat | 🥩 | #FFCDD2 | 3 |
| `aaaaaaaa-4444-...` | Bakery | 🍞 | #FFE0B2 | 4 |
| `aaaaaaaa-5555-...` | Beverages | 🥤 | #F3E5F5 | 5 |
| `aaaaaaaa-6666-...` | Snacks | 🍪 | #FFF9C4 | 6 |
| `aaaaaaaa-7777-...` | Frozen | ❄️ | #E0F7FA | 7 |
| `aaaaaaaa-8888-...` | Household | 🧹 | #F5F5F5 | 8 |
| `aaaaaaaa-9999-...` | Personal Care | 🧴 | #FCE4EC | 9 |
| `aaaaaaaa-0000-...` | Other | 📦 | #ECEFF1 | 999 |

---

## 6. `list_items`

Individual items within shopping lists.

### Schema

| Column Name | Type | Constraints | Description |
|------------|------|-------------|-------------|
| `id` | GUID | PRIMARY KEY | Unique item identifier |
| `list_id` | GUID | FK → lists.id, NOT NULL | Parent list |
| `name` | VARCHAR(200) | NOT NULL | Item name |
| `quantity` | DECIMAL(10,2) | NOT NULL, DEFAULT 1.0 | Quantity to purchase |
| `unit` | VARCHAR(50) | NULL | Unit of measurement (lbs, oz, ea, etc.) |
| `category_id` | GUID | FK → categories.id, NOT NULL | Item category |
| `notes` | TEXT | NULL | Additional notes/specifications |
| `image_url` | VARCHAR(500) | NULL | URL to item image |
| `is_purchased` | BOOLEAN | NOT NULL, DEFAULT false | Purchase status |
| `purchased_at` | DATETIME | NULL | Timestamp when marked purchased |
| `purchased_by` | GUID | FK → users.id | User who purchased |
| `position` | INT | NOT NULL | Display order within list |
| `created_by` | GUID | FK → users.id | User who added item |
| `created_at` | DATETIME | NOT NULL | Record creation timestamp |
| `modified_by` | GUID | FK → users.id | Last modifier |
| `modified_at` | DATETIME | NULL | Last modification timestamp |
| `deleted_by` | GUID | FK → users.id | User who soft-deleted |
| `deleted_at` | DATETIME | NULL | Soft delete timestamp |
| `is_deleted` | BOOLEAN | NOT NULL, DEFAULT false | Soft delete flag |

### Indexes
```sql
CREATE INDEX idx_list_items_list_id ON list_items(list_id) WHERE is_deleted = false;
CREATE INDEX idx_list_items_category_id ON list_items(category_id) WHERE is_deleted = false;
CREATE INDEX idx_list_items_is_purchased ON list_items(is_purchased) WHERE is_deleted = false;
CREATE INDEX idx_list_items_position ON list_items(list_id, position) WHERE is_deleted = false;
CREATE INDEX idx_list_items_created_by ON list_items(created_by) WHERE is_deleted = false;
CREATE INDEX idx_list_items_is_deleted ON list_items(is_deleted);
CREATE INDEX idx_list_items_name ON list_items(name) WHERE is_deleted = false; -- For search
```

### Constraints
```sql
CHECK (LENGTH(name) >= 1 AND LENGTH(name) <= 200)
CHECK (quantity > 0)
CHECK (position >= 0)
CHECK ((is_purchased = true AND purchased_at IS NOT NULL AND purchased_by IS NOT NULL) 
       OR (is_purchased = false AND purchased_at IS NULL AND purchased_by IS NULL))
```

### Business Rules
- Position is auto-assigned as MAX(position) + 1 when creating new items
- Marking as purchased sets `is_purchased = true`, `purchased_at = CURRENT_TIMESTAMP`, `purchased_by = current_user_id`
- Unmarking purchased clears all purchased-related fields
- Soft-deleting list cascades to soft-delete all items

---

## Relationships Summary

```
users (1) ──→ (M) lists [owner_id]
users (1) ──→ (M) user_to_list [user_id]
users (1) ──→ (M) list_items [created_by, purchased_by]
users (1) ──→ (M) categories [custom_owner_id]

lists (1) ──→ (M) list_items [list_id]
lists (1) ──→ (M) user_to_list [list_id]

roles (1) ──→ (M) user_to_list [role_id]

categories (1) ──→ (M) list_items [category_id]
```

---

## Entity Relationship Diagram

```
┌─────────────────┐
│     users       │
│─────────────────│
│ id (PK)         │──┐
│ email           │  │
│ display_name    │  │
│ password_hash   │  │
│ avatar_url      │  │
│ ...audit cols   │  │
└─────────────────┘  │
         │           │
         │ owner_id  │
         ▼           │
┌─────────────────┐  │
│     lists       │  │
│─────────────────│  │
│ id (PK)         │──┼──┐
│ name            │  │  │
│ description     │  │  │
│ owner_id (FK)   │──┘  │
│ is_archived     │     │
│ ...audit cols   │     │
└─────────────────┘     │
         │              │ list_id
         │ list_id      │
         ▼              │
┌─────────────────┐     │
│   list_items    │     │
│─────────────────│     │
│ id (PK)         │     │
│ list_id (FK)    │─────┘
│ name            │
│ quantity        │     ┌─────────────────┐
│ category_id(FK) │────→│   categories    │
│ is_purchased    │     │─────────────────│
│ purchased_by(FK)│──┐  │ id (PK)         │
│ position        │  │  │ name            │
│ ...audit cols   │  │  │ icon            │
└─────────────────┘  │  │ color           │
                     │  │ is_default      │
         ┌───────────┘  │ ...audit cols   │
         │              └─────────────────┘
         ▼
┌─────────────────┐
│  user_to_list   │
│─────────────────│
│ id (PK)         │
│ list_id (FK)    │     ┌─────────────────┐
│ user_id (FK)    │     │     roles       │
│ role_id (FK)    │────→│─────────────────│
│ invited_by (FK) │     │ id (PK)         │
│ is_pending      │     │ name            │
│ accepted_at     │     │ can_*           │
│ ...audit cols   │     │ ...audit cols   │
└─────────────────┘     └─────────────────┘
```

---

## Seed Data Requirements

### System User
```sql
INSERT INTO users (id, email, display_name, password_hash, created_by, created_at, is_deleted)
VALUES (
    '00000000-0000-0000-0000-000000000000',
    'system@ae-infinity.local',
    'System',
    'NOT_APPLICABLE',
    '00000000-0000-0000-0000-000000000000',
    CURRENT_TIMESTAMP,
    false
);
```

### Default Roles
Must seed Owner, Editor, Editor-Limited, and Viewer roles as specified above.

### Default Categories
Must seed 10 default categories as specified in categories table.

---

## Migration Strategy

### Initial Migration (v1)
1. Create all tables in order:
   - users (no FK dependencies)
   - roles (no FK dependencies)
   - categories (FK to users)
   - lists (FK to users)
   - list_items (FK to lists, categories, users)
   - user_to_list (FK to lists, users, roles)

2. Add indexes

3. Add constraints

4. Seed data:
   - System user
   - Default roles
   - Default categories

### Future Migration Considerations
- Add `list_templates` table for reusable lists
- Add `item_history` table for purchase tracking
- Add `list_sharing_links` table for shareable URLs
- Add `notifications` table for user alerts
- Add `user_preferences` table for settings

---

## Performance Optimization

### Query Optimization
- **Always filter by `is_deleted = false`** in application queries
- Use covering indexes for frequently queried combinations
- Partition large tables by date if needed (list_items, user_to_list)

### Caching Strategy
- Cache active user sessions (Redis)
- Cache list collaborators for permission checks
- Cache default categories
- Cache user profiles

### Archival Strategy
- Consider moving records deleted > 90 days to archive tables
- Maintain foreign key relationships for audit purposes
- Implement data retention policies per legal requirements

---

## Security Considerations

### Data Protection
- **Never return password_hash** in API responses
- **Never return soft-deleted records** unless explicitly authorized
- **Hash all passwords** using bcrypt or Argon2 (min 12 rounds)
- **Validate all user input** before database operations

### Access Control
- Verify user permissions through `user_to_list` table
- Owner check: `owner_id = current_user_id`
- Collaborator check: JOIN through `user_to_list` with role validation
- Prevent privilege escalation: Only owners can grant Owner role

### Audit Trail
- All audit columns must be populated on every operation
- Audit logs should be immutable
- Consider separate `audit_log` table for sensitive operations
- Retain deleted records for compliance (GDPR, data retention laws)

---

## Testing Data

Provide realistic test data including:
- 5 test users with different roles
- 3-5 shopping lists with various collaboration scenarios
- 20-30 items across different categories and lists
- Mix of purchased and unpurchased items
- Active and archived lists
- Pending and accepted collaborations

---

This schema provides a solid foundation for the AE Infinity shopping list application with proper audit trails, soft delete capabilities, and scalable collaboration features.

