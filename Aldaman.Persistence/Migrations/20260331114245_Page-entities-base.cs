using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldaman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Pageentitiesbase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PageDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PageKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RouteSegment = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsHomePage = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DefaultSortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PageContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PageDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CultureCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SeoTitle = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SeoDescription = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    SectionsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PublishedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageContents_PageDefinitions_PageDefinitionId",
                        column: x => x.PageDefinitionId,
                        principalTable: "PageDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageContents_PageDefinitionId_CultureCode",
                table: "PageContents",
                columns: new[] { "PageDefinitionId", "CultureCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageDefinitions_PageKey",
                table: "PageDefinitions",
                column: "PageKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageContents");

            migrationBuilder.DropTable(
                name: "PageDefinitions");
        }
    }
}
