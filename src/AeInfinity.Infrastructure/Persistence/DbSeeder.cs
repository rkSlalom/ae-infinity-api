using AeInfinity.Domain.Entities;
using AeInfinity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Infrastructure.Persistence;

/// <summary>
/// Seeds the database with initial data for development and testing
/// </summary>
public class DbSeeder
{
    private readonly ApplicationDbContext _context;
    private static readonly DateTime SeedDate = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // Consistent GUIDs for seed data (for testing purposes)
    private static readonly Guid OwnerRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid EditorRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid EditorLimitedRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid ViewerRoleId = Guid.Parse("44444444-4444-4444-4444-444444444444");

    private static readonly Guid SarahUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid MikeUserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Guid EmmaUserId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    private static readonly Guid WeeklyListId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    private static readonly Guid PartyListId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

    public DbSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAllAsync()
    {
        // Check if database already has data
        if (await _context.Roles.AnyAsync())
        {
            return; // Database already seeded
        }

        await SeedRolesAsync();
        await SeedCategoriesAsync();
        await SeedUsersAsync();
        await SeedListsAsync();
        await SeedCollaborationsAsync();
        await SeedListItemsAsync();

        await _context.SaveChangesAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roles = new List<Role>
        {
            new()
            {
                Id = OwnerRoleId,
                Name = "Owner",
                Description = "Full control over the list including deletion and managing collaborators",
                CanCreateItems = true,
                CanEditItems = true,
                CanDeleteItems = true,
                CanEditOwnItemsOnly = false,
                CanMarkPurchased = true,
                CanEditListDetails = true,
                CanManageCollaborators = true,
                CanDeleteList = true,
                CanArchiveList = true,
                PriorityOrder = 1,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new()
            {
                Id = EditorRoleId,
                Name = "Editor",
                Description = "Can add, edit, delete items and mark as purchased. Cannot manage list or collaborators",
                CanCreateItems = true,
                CanEditItems = true,
                CanDeleteItems = true,
                CanEditOwnItemsOnly = false,
                CanMarkPurchased = true,
                CanEditListDetails = false,
                CanManageCollaborators = false,
                CanDeleteList = false,
                CanArchiveList = false,
                PriorityOrder = 2,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new()
            {
                Id = EditorLimitedRoleId,
                Name = "Editor-Limited",
                Description = "Can add items and edit only their own items. Cannot delete items created by others",
                CanCreateItems = true,
                CanEditItems = true,
                CanDeleteItems = true,
                CanEditOwnItemsOnly = true,
                CanMarkPurchased = true,
                CanEditListDetails = false,
                CanManageCollaborators = false,
                CanDeleteList = false,
                CanArchiveList = false,
                PriorityOrder = 3,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new()
            {
                Id = ViewerRoleId,
                Name = "Viewer",
                Description = "Read-only access to the list. Cannot make any changes",
                CanCreateItems = false,
                CanEditItems = false,
                CanDeleteItems = false,
                CanEditOwnItemsOnly = false,
                CanMarkPurchased = false,
                CanEditListDetails = false,
                CanManageCollaborators = false,
                CanDeleteList = false,
                CanArchiveList = false,
                PriorityOrder = 4,
                CreatedAt = SeedDate,
                IsDeleted = false
            }
        };

        await _context.Roles.AddRangeAsync(roles);
    }

    private async Task SeedCategoriesAsync()
    {
        var categories = new List<Category>
        {
            new()
            {
                Id = Guid.Parse("c1111111-1111-1111-1111-111111111111"),
                Name = "Produce",
                Icon = "🥬",
                Color = "#4CAF50",
                IsDefault = true,
                IsCustom = false,
                SortOrder = 1,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.Parse("c2222222-2222-2222-2222-222222222222"),
                Name = "Dairy",
                Icon = "🥛",
                Color = "#2196F3",
                IsDefault = true,
                IsCustom = false,
                SortOrder = 2,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.Parse("c3333333-3333-3333-3333-333333333333"),
                Name = "Meat & Seafood",
                Icon = "🥩",
                Color = "#F44336",
                IsDefault = true,
                IsCustom = false,
                SortOrder = 3,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.Parse("c4444444-4444-4444-4444-444444444444"),
                Name = "Bakery",
                Icon = "🍞",
                Color = "#FF9800",
                IsDefault = true,
                IsCustom = false,
                SortOrder = 4,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.Parse("c5555555-5555-5555-5555-555555555555"),
                Name = "Beverages",
                Icon = "☕",
                Color = "#795548",
                IsDefault = true,
                IsCustom = false,
                SortOrder = 5,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.Parse("c6666666-6666-6666-6666-666666666666"),
                Name = "Snacks",
                Icon = "🍿",
                Color = "#FFC107",
                IsDefault = true,
                IsCustom = false,
                SortOrder = 6,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.Parse("c7777777-7777-7777-7777-777777777777"),
                Name = "Frozen",
                Icon = "🧊",
                Color = "#00BCD4",
                IsDefault = true,
                IsCustom = false,
                SortOrder = 7,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.Parse("c8888888-8888-8888-8888-888888888888"),
                Name = "Household",
                Icon = "🧹",
                Color = "#9C27B0",
                IsDefault = true,
                IsCustom = false,
                SortOrder = 8,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.Parse("c9999999-9999-9999-9999-999999999999"),
                Name = "Personal Care",
                Icon = "🧴",
                Color = "#E91E63",
                IsDefault = true,
                IsCustom = false,
                SortOrder = 9,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.Parse("c0000000-0000-0000-0000-000000000000"),
                Name = "Other",
                Icon = "📦",
                Color = "#607D8B",
                IsDefault = true,
                IsCustom = false,
                SortOrder = 10,
                CreatedAt = SeedDate,
                IsDeleted = false
            }
        };

        await _context.Categories.AddRangeAsync(categories);
    }

    private async Task SeedUsersAsync()
    {
        var passwordHash = PasswordHasher.HashPassword("Password123!");

        var users = new List<User>
        {
            new()
            {
                Id = SarahUserId,
                Email = "sarah@example.com",
                EmailNormalized = "SARAH@EXAMPLE.COM",
                DisplayName = "Sarah Johnson",
                PasswordHash = passwordHash,
                IsEmailVerified = true,
                LastLoginAt = SeedDate,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new()
            {
                Id = MikeUserId,
                Email = "mike@example.com",
                EmailNormalized = "MIKE@EXAMPLE.COM",
                DisplayName = "Mike Chen",
                PasswordHash = passwordHash,
                IsEmailVerified = true,
                LastLoginAt = SeedDate,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new()
            {
                Id = EmmaUserId,
                Email = "emma@example.com",
                EmailNormalized = "EMMA@EXAMPLE.COM",
                DisplayName = "Emma Davis",
                PasswordHash = passwordHash,
                IsEmailVerified = true,
                LastLoginAt = SeedDate,
                CreatedAt = SeedDate,
                IsDeleted = false
            }
        };

        await _context.Users.AddRangeAsync(users);
    }

    private async Task SeedListsAsync()
    {
        var lists = new List<List>
        {
            new()
            {
                Id = WeeklyListId,
                Name = "Weekly Groceries",
                Description = "Our regular weekly shopping list",
                OwnerId = SarahUserId,
                IsArchived = false,
                CreatedAt = SeedDate,
                CreatedBy = SarahUserId,
                IsDeleted = false
            },
            new()
            {
                Id = PartyListId,
                Name = "Birthday Party Supplies",
                Description = "Items needed for Emma's birthday party",
                OwnerId = MikeUserId,
                IsArchived = false,
                CreatedAt = SeedDate,
                CreatedBy = MikeUserId,
                IsDeleted = false
            }
        };

        await _context.Lists.AddRangeAsync(lists);
    }

    private async Task SeedCollaborationsAsync()
    {
        var collaborations = new List<UserToList>
        {
            // Weekly Groceries: Sarah (Owner), Mike (Editor), Emma (Viewer)
            new()
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                ListId = WeeklyListId,
                UserId = SarahUserId,
                RoleId = OwnerRoleId,
                InvitedBy = SarahUserId,
                InvitedAt = SeedDate,
                AcceptedAt = SeedDate,
                IsPending = false,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                ListId = WeeklyListId,
                UserId = MikeUserId,
                RoleId = EditorRoleId,
                InvitedBy = SarahUserId,
                InvitedAt = SeedDate,
                AcceptedAt = SeedDate,
                IsPending = false,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                ListId = WeeklyListId,
                UserId = EmmaUserId,
                RoleId = ViewerRoleId,
                InvitedBy = SarahUserId,
                InvitedAt = SeedDate,
                AcceptedAt = SeedDate,
                IsPending = false,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            // Birthday Party: Mike (Owner), Sarah (Editor)
            new()
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                ListId = PartyListId,
                UserId = MikeUserId,
                RoleId = OwnerRoleId,
                InvitedBy = MikeUserId,
                InvitedAt = SeedDate,
                AcceptedAt = SeedDate,
                IsPending = false,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                ListId = PartyListId,
                UserId = SarahUserId,
                RoleId = EditorRoleId,
                InvitedBy = MikeUserId,
                InvitedAt = SeedDate,
                AcceptedAt = SeedDate,
                IsPending = false,
                CreatedAt = SeedDate,
                IsDeleted = false
            }
        };

        await _context.UserToLists.AddRangeAsync(collaborations);
    }

    private async Task SeedListItemsAsync()
    {
        var items = new List<ListItem>
        {
            // Weekly Groceries Items
            new()
            {
                Id = Guid.NewGuid(),
                ListId = WeeklyListId,
                Name = "Organic Spinach",
                Quantity = 2,
                Unit = "bunches",
                CategoryId = Guid.Parse("c1111111-1111-1111-1111-111111111111"), // Produce
                Notes = "Fresh, not frozen",
                IsPurchased = false,
                Position = 1,
                CreatedAt = SeedDate,
                CreatedBy = SarahUserId,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                ListId = WeeklyListId,
                Name = "Whole Milk",
                Quantity = 1,
                Unit = "gallon",
                CategoryId = Guid.Parse("c2222222-2222-2222-2222-222222222222"), // Dairy
                IsPurchased = true,
                PurchasedAt = SeedDate.AddDays(1),
                PurchasedBy = MikeUserId,
                Position = 2,
                CreatedAt = SeedDate,
                CreatedBy = SarahUserId,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                ListId = WeeklyListId,
                Name = "Chicken Breast",
                Quantity = 2,
                Unit = "lbs",
                CategoryId = Guid.Parse("c3333333-3333-3333-3333-333333333333"), // Meat
                Notes = "Boneless, skinless",
                IsPurchased = false,
                Position = 3,
                CreatedAt = SeedDate,
                CreatedBy = SarahUserId,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                ListId = WeeklyListId,
                Name = "Whole Wheat Bread",
                Quantity = 1,
                Unit = "loaf",
                CategoryId = Guid.Parse("c4444444-4444-4444-4444-444444444444"), // Bakery
                IsPurchased = false,
                Position = 4,
                CreatedAt = SeedDate,
                CreatedBy = MikeUserId,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                ListId = WeeklyListId,
                Name = "Orange Juice",
                Quantity = 1,
                Unit = "half gallon",
                CategoryId = Guid.Parse("c5555555-5555-5555-5555-555555555555"), // Beverages
                Notes = "Pulp-free",
                IsPurchased = true,
                PurchasedAt = SeedDate.AddDays(1),
                PurchasedBy = MikeUserId,
                Position = 5,
                CreatedAt = SeedDate,
                CreatedBy = SarahUserId,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                ListId = WeeklyListId,
                Name = "Tortilla Chips",
                Quantity = 2,
                Unit = "bags",
                CategoryId = Guid.Parse("c6666666-6666-6666-6666-666666666666"), // Snacks
                IsPurchased = false,
                Position = 6,
                CreatedAt = SeedDate,
                CreatedBy = MikeUserId,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                ListId = WeeklyListId,
                Name = "Frozen Pizza",
                Quantity = 3,
                Unit = "boxes",
                CategoryId = Guid.Parse("c7777777-7777-7777-7777-777777777777"), // Frozen
                Notes = "Pepperoni flavor",
                IsPurchased = false,
                Position = 7,
                CreatedAt = SeedDate,
                CreatedBy = SarahUserId,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                ListId = WeeklyListId,
                Name = "Paper Towels",
                Quantity = 1,
                Unit = "pack",
                CategoryId = Guid.Parse("c8888888-8888-8888-8888-888888888888"), // Household
                IsPurchased = false,
                Position = 8,
                CreatedAt = SeedDate,
                CreatedBy = SarahUserId,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                ListId = WeeklyListId,
                Name = "Shampoo",
                Quantity = 1,
                Unit = "bottle",
                CategoryId = Guid.Parse("c9999999-9999-9999-9999-999999999999"), // Personal Care
                Notes = "For dry hair",
                IsPurchased = false,
                Position = 9,
                CreatedAt = SeedDate,
                CreatedBy = SarahUserId,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                ListId = WeeklyListId,
                Name = "Batteries",
                Quantity = 1,
                Unit = "pack",
                CategoryId = Guid.Parse("c0000000-0000-0000-0000-000000000000"), // Other
                Notes = "AA size",
                IsPurchased = false,
                Position = 10,
                CreatedAt = SeedDate,
                CreatedBy = MikeUserId,
                IsDeleted = false
            },
            // Birthday Party Items
            new()
            {
                Id = Guid.NewGuid(),
                ListId = PartyListId,
                Name = "Birthday Cake",
                Quantity = 1,
                Unit = "cake",
                CategoryId = Guid.Parse("c4444444-4444-4444-4444-444444444444"), // Bakery
                Notes = "Chocolate with vanilla frosting",
                IsPurchased = false,
                Position = 1,
                CreatedAt = SeedDate,
                CreatedBy = MikeUserId,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                ListId = PartyListId,
                Name = "Soda",
                Quantity = 3,
                Unit = "2-liter bottles",
                CategoryId = Guid.Parse("c5555555-5555-5555-5555-555555555555"), // Beverages
                Notes = "Mixed flavors",
                IsPurchased = true,
                PurchasedAt = SeedDate.AddDays(1),
                PurchasedBy = SarahUserId,
                Position = 2,
                CreatedAt = SeedDate,
                CreatedBy = MikeUserId,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                ListId = PartyListId,
                Name = "Potato Chips",
                Quantity = 4,
                Unit = "bags",
                CategoryId = Guid.Parse("c6666666-6666-6666-6666-666666666666"), // Snacks
                IsPurchased = false,
                Position = 3,
                CreatedAt = SeedDate,
                CreatedBy = MikeUserId,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                ListId = PartyListId,
                Name = "Ice Cream",
                Quantity = 2,
                Unit = "gallons",
                CategoryId = Guid.Parse("c7777777-7777-7777-7777-777777777777"), // Frozen
                Notes = "Vanilla and chocolate",
                IsPurchased = false,
                Position = 4,
                CreatedAt = SeedDate,
                CreatedBy = SarahUserId,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                ListId = PartyListId,
                Name = "Paper Plates",
                Quantity = 2,
                Unit = "packs",
                CategoryId = Guid.Parse("c8888888-8888-8888-8888-888888888888"), // Household
                IsPurchased = false,
                Position = 5,
                CreatedAt = SeedDate,
                CreatedBy = MikeUserId,
                IsDeleted = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                ListId = PartyListId,
                Name = "Party Balloons",
                Quantity = 1,
                Unit = "pack",
                CategoryId = Guid.Parse("c0000000-0000-0000-0000-000000000000"), // Other
                Notes = "Assorted colors",
                IsPurchased = true,
                PurchasedAt = SeedDate.AddDays(1),
                PurchasedBy = SarahUserId,
                Position = 6,
                CreatedAt = SeedDate,
                CreatedBy = MikeUserId,
                IsDeleted = false
            }
        };

        await _context.ListItems.AddRangeAsync(items);
    }
}

