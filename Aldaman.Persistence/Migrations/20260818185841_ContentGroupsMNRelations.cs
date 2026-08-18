using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldaman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ContentGroupsMNRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentGroupBlogPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlogPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentGroupBlogPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentGroupBlogPosts_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentGroupBlogPosts_AspNetUsers_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentGroupBlogPosts_BlogPosts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentGroupBlogPosts_ContentGroups_ContentGroupId",
                        column: x => x.ContentGroupId,
                        principalTable: "ContentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContentGroupContentPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentGroupContentPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentGroupContentPages_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentGroupContentPages_AspNetUsers_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentGroupContentPages_ContentGroups_ContentGroupId",
                        column: x => x.ContentGroupId,
                        principalTable: "ContentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentGroupContentPages_ContentPages_ContentPageId",
                        column: x => x.ContentPageId,
                        principalTable: "ContentPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentGroupBlogPosts_BlogPostId",
                table: "ContentGroupBlogPosts",
                column: "BlogPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentGroupBlogPosts_ContentGroupId_BlogPostId",
                table: "ContentGroupBlogPosts",
                columns: new[] { "ContentGroupId", "BlogPostId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentGroupBlogPosts_CreatedByUserId",
                table: "ContentGroupBlogPosts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentGroupBlogPosts_UpdatedByUserId",
                table: "ContentGroupBlogPosts",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentGroupContentPages_ContentGroupId_ContentPageId",
                table: "ContentGroupContentPages",
                columns: new[] { "ContentGroupId", "ContentPageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentGroupContentPages_ContentPageId",
                table: "ContentGroupContentPages",
                column: "ContentPageId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentGroupContentPages_CreatedByUserId",
                table: "ContentGroupContentPages",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentGroupContentPages_UpdatedByUserId",
                table: "ContentGroupContentPages",
                column: "UpdatedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentGroupBlogPosts");

            migrationBuilder.DropTable(
                name: "ContentGroupContentPages");
        }
    }
}
