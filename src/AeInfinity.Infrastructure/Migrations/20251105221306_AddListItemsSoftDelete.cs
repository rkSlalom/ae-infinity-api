using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeInfinity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddListItemsSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CanCreateItems = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanEditItems = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanDeleteItems = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanEditOwnItemsOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanMarkPurchased = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanEditListDetails = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanManageCollaborators = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanDeleteList = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanArchiveList = table.Column<bool>(type: "INTEGER", nullable: false),
                    PriorityOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    EmailNormalized = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    AvatarUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsEmailVerified = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    EmailVerificationToken = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    PasswordResetToken = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    PasswordResetExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 7, nullable: true),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    IsCustom = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    CustomOwnerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 999),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Users_CustomOwnerId",
                        column: x => x.CustomOwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Lists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    OwnerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lists_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ListItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ListId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    PurchasedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 1m),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    IsPurchased = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedById = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PurchasedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CategoryId1 = table.Column<Guid>(type: "TEXT", nullable: true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListItems_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ListItems_Categories_CategoryId1",
                        column: x => x.CategoryId1,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ListItems_Lists_ListId",
                        column: x => x.ListId,
                        principalTable: "Lists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListItems_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListItems_Users_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ListItems_Users_PurchasedBy",
                        column: x => x.PurchasedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ListItems_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ListItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserToLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ListId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InvitedBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    InvitedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsPending = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserToLists_Lists_ListId",
                        column: x => x.ListId,
                        principalTable: "Lists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserToLists_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserToLists_Users_InvitedBy",
                        column: x => x.InvitedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserToLists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CustomOwnerId",
                table: "Categories",
                column: "CustomOwnerId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IsDefault",
                table: "Categories",
                column: "IsDefault",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IsDeleted",
                table: "Categories",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_SortOrder",
                table: "Categories",
                column: "SortOrder",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ListItems_CategoryId",
                table: "ListItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ListItems_CategoryId1",
                table: "ListItems",
                column: "CategoryId1");

            migrationBuilder.CreateIndex(
                name: "IX_ListItems_CreatedBy_Name",
                table: "ListItems",
                columns: new[] { "CreatedBy", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_ListItems_DeletedById",
                table: "ListItems",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ListItems_IsDeleted",
                table: "ListItems",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ListItems_ListId",
                table: "ListItems",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_ListItems_ListId_CategoryId",
                table: "ListItems",
                columns: new[] { "ListId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_ListItems_ListId_IsPurchased",
                table: "ListItems",
                columns: new[] { "ListId", "IsPurchased" });

            migrationBuilder.CreateIndex(
                name: "IX_ListItems_ListId_Position",
                table: "ListItems",
                columns: new[] { "ListId", "Position" });

            migrationBuilder.CreateIndex(
                name: "IX_ListItems_PurchasedBy",
                table: "ListItems",
                column: "PurchasedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ListItems_UpdatedBy",
                table: "ListItems",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ListItems_UserId",
                table: "ListItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Lists_CreatedAt",
                table: "Lists",
                column: "CreatedAt",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Lists_IsArchived",
                table: "Lists",
                column: "IsArchived",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Lists_IsDeleted",
                table: "Lists",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Lists_Name",
                table: "Lists",
                column: "Name",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Lists_OwnerId",
                table: "Lists",
                column: "OwnerId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_IsDeleted",
                table: "Roles",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailNormalized",
                table: "Users",
                column: "EmailNormalized",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsDeleted",
                table: "Users",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserToLists_InvitedBy",
                table: "UserToLists",
                column: "InvitedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserToLists_IsDeleted",
                table: "UserToLists",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserToLists_IsPending",
                table: "UserToLists",
                column: "IsPending",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_UserToLists_ListId",
                table: "UserToLists",
                column: "ListId",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_UserToLists_ListId_UserId",
                table: "UserToLists",
                columns: new[] { "ListId", "UserId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_UserToLists_RoleId",
                table: "UserToLists",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToLists_UserId",
                table: "UserToLists",
                column: "UserId",
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListItems");

            migrationBuilder.DropTable(
                name: "UserToLists");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Lists");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
