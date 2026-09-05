using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldaman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ContentEntityUnificationMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogPostTranslations");

            migrationBuilder.DropTable(
                name: "ContentGroupBlogPosts");

            migrationBuilder.DropTable(
                name: "ContentGroupContentPages");

            migrationBuilder.DropTable(
                name: "ContentPageTranslations");

            migrationBuilder.DropTable(
                name: "BlogPosts");

            migrationBuilder.DropTable(
                name: "ContentPages");

            migrationBuilder.CreateTable(
                name: "Contents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlaceToShow = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CoverMediaAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PublishedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
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
                    table.PrimaryKey("PK_Contents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contents_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contents_AspNetUsers_DeletedByUserId",
                        column: x => x.DeletedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contents_AspNetUsers_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contents_MediaAssets_CoverMediaAssetId",
                        column: x => x.CoverMediaAssetId,
                        principalTable: "MediaAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContentGroupContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentGroupContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentGroupContents_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentGroupContents_AspNetUsers_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentGroupContents_ContentGroups_ContentGroupId",
                        column: x => x.ContentGroupId,
                        principalTable: "ContentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentGroupContents_Contents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Contents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContentTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CultureCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DisplayTitle = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Slug = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Perex = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    BodyHtml = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyDeltaJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlainText = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    DisplayExpanded = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentTranslations_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentTranslations_AspNetUsers_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentTranslations_Contents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Contents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentGroupContents_ContentGroupId_ContentId",
                table: "ContentGroupContents",
                columns: new[] { "ContentGroupId", "ContentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentGroupContents_ContentId",
                table: "ContentGroupContents",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentGroupContents_CreatedByUserId",
                table: "ContentGroupContents",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentGroupContents_UpdatedByUserId",
                table: "ContentGroupContents",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CoverMediaAssetId",
                table: "Contents",
                column: "CoverMediaAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CreatedByUserId",
                table: "Contents",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_DeletedByUserId",
                table: "Contents",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_IsPublished",
                table: "Contents",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_Order",
                table: "Contents",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_PlaceToShow",
                table: "Contents",
                column: "PlaceToShow");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_PublishedAtUtc",
                table: "Contents",
                column: "PublishedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_UpdatedByUserId",
                table: "Contents",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTranslations_ContentId_CultureCode",
                table: "ContentTranslations",
                columns: new[] { "ContentId", "CultureCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentTranslations_CreatedByUserId",
                table: "ContentTranslations",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTranslations_CultureCode_Slug",
                table: "ContentTranslations",
                columns: new[] { "CultureCode", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentTranslations_UpdatedByUserId",
                table: "ContentTranslations",
                column: "UpdatedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentGroupContents");

            migrationBuilder.DropTable(
                name: "ContentTranslations");

            migrationBuilder.DropTable(
                name: "Contents");

            migrationBuilder.CreateTable(
                name: "BlogPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoverMediaAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PublishedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogPosts_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlogPosts_AspNetUsers_DeletedByUserId",
                        column: x => x.DeletedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlogPosts_AspNetUsers_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlogPosts_MediaAssets_CoverMediaAssetId",
                        column: x => x.CoverMediaAssetId,
                        principalTable: "MediaAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContentPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    PageOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    PlaceToShow = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "BlogPostTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlogPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BodyDeltaJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyHtml = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CultureCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    DisplayExpanded = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Perex = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    PlainText = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPostTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogPostTranslations_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlogPostTranslations_AspNetUsers_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlogPostTranslations_BlogPosts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentGroupBlogPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlogPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "ContentPageTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BodyDeltaJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyHtml = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CultureCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    DisplayTitle = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PlainText = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_CoverMediaAssetId",
                table: "BlogPosts",
                column: "CoverMediaAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_CreatedByUserId",
                table: "BlogPosts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_DeletedByUserId",
                table: "BlogPosts",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_IsPublished",
                table: "BlogPosts",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_PublishedAtUtc",
                table: "BlogPosts",
                column: "PublishedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_UpdatedByUserId",
                table: "BlogPosts",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTranslations_BlogPostId_CultureCode",
                table: "BlogPostTranslations",
                columns: new[] { "BlogPostId", "CultureCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTranslations_CreatedByUserId",
                table: "BlogPostTranslations",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTranslations_Slug",
                table: "BlogPostTranslations",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTranslations_UpdatedByUserId",
                table: "BlogPostTranslations",
                column: "UpdatedByUserId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ContentPages_CreatedByUserId",
                table: "ContentPages",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentPages_DeletedByUserId",
                table: "ContentPages",
                column: "DeletedByUserId");

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
                name: "IX_ContentPageTranslations_UpdatedByUserId",
                table: "ContentPageTranslations",
                column: "UpdatedByUserId");
        }
    }
}
