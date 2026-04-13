using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldaman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamePageEntitiesToContentPageEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageContents");

            migrationBuilder.DropTable(
                name: "PageDefinitions");

            migrationBuilder.CreateTable(
                name: "ContentPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PageKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ShowOnHomePage = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PageOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    DefaultSortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentPages_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentPages_AspNetUsers_DeletedByUserId",
                        column: x => x.DeletedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentPages_AspNetUsers_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContentPageTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CultureCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPageTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentPageTranslations_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentPageTranslations_AspNetUsers_DeletedByUserId",
                        column: x => x.DeletedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentPageTranslations_AspNetUsers_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentPageTranslations_ContentPages_ContentPageId",
                        column: x => x.ContentPageId,
                        principalTable: "ContentPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentPages_CreatedByUserId",
                table: "ContentPages",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentPages_DeletedByUserId",
                table: "ContentPages",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentPages_PageKey",
                table: "ContentPages",
                column: "PageKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentPages_UpdatedByUserId",
                table: "ContentPages",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentPageTranslations_ContentPageId_CultureCode",
                table: "ContentPageTranslations",
                columns: new[] { "ContentPageId", "CultureCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentPageTranslations_CreatedByUserId",
                table: "ContentPageTranslations",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentPageTranslations_DeletedByUserId",
                table: "ContentPageTranslations",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentPageTranslations_UpdatedByUserId",
                table: "ContentPageTranslations",
                column: "UpdatedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentPageTranslations");

            migrationBuilder.DropTable(
                name: "ContentPages");

            migrationBuilder.CreateTable(
                name: "PageDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DefaultSortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    PageKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PageOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ShowOnHomePage = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageDefinitions_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PageDefinitions_AspNetUsers_DeletedByUserId",
                        column: x => x.DeletedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PageDefinitions_AspNetUsers_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PageContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PageDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CultureCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageContents_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PageContents_AspNetUsers_DeletedByUserId",
                        column: x => x.DeletedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PageContents_AspNetUsers_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PageContents_PageDefinitions_PageDefinitionId",
                        column: x => x.PageDefinitionId,
                        principalTable: "PageDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageContents_CreatedByUserId",
                table: "PageContents",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PageContents_DeletedByUserId",
                table: "PageContents",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PageContents_PageDefinitionId_CultureCode",
                table: "PageContents",
                columns: new[] { "PageDefinitionId", "CultureCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageContents_UpdatedByUserId",
                table: "PageContents",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PageDefinitions_CreatedByUserId",
                table: "PageDefinitions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PageDefinitions_DeletedByUserId",
                table: "PageDefinitions",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PageDefinitions_PageKey",
                table: "PageDefinitions",
                column: "PageKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageDefinitions_UpdatedByUserId",
                table: "PageDefinitions",
                column: "UpdatedByUserId");
        }
    }
}
