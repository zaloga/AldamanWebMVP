using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldaman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesCRUDUnification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_AspNetUsers_DeletedByUserId",
                table: "BlogPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_AspNetUsers_CreatedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_AspNetUsers_DeletedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_AspNetUsers_UpdatedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaAssets_AspNetUsers_DeletedByUserId",
                table: "MediaAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaAssets_AspNetUsers_UpdatedByUserId",
                table: "MediaAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_PageContents_AspNetUsers_CreatedByUserId",
                table: "PageContents");

            migrationBuilder.DropForeignKey(
                name: "FK_PageContents_AspNetUsers_DeletedByUserId",
                table: "PageContents");

            migrationBuilder.DropForeignKey(
                name: "FK_PageContents_AspNetUsers_UpdatedByUserId",
                table: "PageContents");

            migrationBuilder.DropForeignKey(
                name: "FK_PageDefinitions_AspNetUsers_CreatedByUserId",
                table: "PageDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_PageDefinitions_AspNetUsers_DeletedByUserId",
                table: "PageDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_PageDefinitions_AspNetUsers_UpdatedByUserId",
                table: "PageDefinitions");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "BlogPostTranslations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "BlogPostTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "BlogPostTranslations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "BlogPostTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "BlogPostTranslations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BlogPostTranslations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "BlogPostTranslations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "BlogPostTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedByUserId",
                table: "BlogPosts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "BlogPosts",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedByUserId",
                table: "BlogPosts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTranslations_CreatedByUserId",
                table: "BlogPostTranslations",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTranslations_DeletedByUserId",
                table: "BlogPostTranslations",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTranslations_UpdatedByUserId",
                table: "BlogPostTranslations",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_AspNetUsers_DeletedByUserId",
                table: "BlogPosts",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostTranslations_AspNetUsers_CreatedByUserId",
                table: "BlogPostTranslations",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostTranslations_AspNetUsers_DeletedByUserId",
                table: "BlogPostTranslations",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostTranslations_AspNetUsers_UpdatedByUserId",
                table: "BlogPostTranslations",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactMessages_AspNetUsers_CreatedByUserId",
                table: "ContactMessages",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactMessages_AspNetUsers_DeletedByUserId",
                table: "ContactMessages",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactMessages_AspNetUsers_UpdatedByUserId",
                table: "ContactMessages",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaAssets_AspNetUsers_DeletedByUserId",
                table: "MediaAssets",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaAssets_AspNetUsers_UpdatedByUserId",
                table: "MediaAssets",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PageContents_AspNetUsers_CreatedByUserId",
                table: "PageContents",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PageContents_AspNetUsers_DeletedByUserId",
                table: "PageContents",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PageContents_AspNetUsers_UpdatedByUserId",
                table: "PageContents",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PageDefinitions_AspNetUsers_CreatedByUserId",
                table: "PageDefinitions",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PageDefinitions_AspNetUsers_DeletedByUserId",
                table: "PageDefinitions",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PageDefinitions_AspNetUsers_UpdatedByUserId",
                table: "PageDefinitions",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_AspNetUsers_DeletedByUserId",
                table: "BlogPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostTranslations_AspNetUsers_CreatedByUserId",
                table: "BlogPostTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostTranslations_AspNetUsers_DeletedByUserId",
                table: "BlogPostTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostTranslations_AspNetUsers_UpdatedByUserId",
                table: "BlogPostTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_AspNetUsers_CreatedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_AspNetUsers_DeletedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_AspNetUsers_UpdatedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaAssets_AspNetUsers_DeletedByUserId",
                table: "MediaAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaAssets_AspNetUsers_UpdatedByUserId",
                table: "MediaAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_PageContents_AspNetUsers_CreatedByUserId",
                table: "PageContents");

            migrationBuilder.DropForeignKey(
                name: "FK_PageContents_AspNetUsers_DeletedByUserId",
                table: "PageContents");

            migrationBuilder.DropForeignKey(
                name: "FK_PageContents_AspNetUsers_UpdatedByUserId",
                table: "PageContents");

            migrationBuilder.DropForeignKey(
                name: "FK_PageDefinitions_AspNetUsers_CreatedByUserId",
                table: "PageDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_PageDefinitions_AspNetUsers_DeletedByUserId",
                table: "PageDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_PageDefinitions_AspNetUsers_UpdatedByUserId",
                table: "PageDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_BlogPostTranslations_CreatedByUserId",
                table: "BlogPostTranslations");

            migrationBuilder.DropIndex(
                name: "IX_BlogPostTranslations_DeletedByUserId",
                table: "BlogPostTranslations");

            migrationBuilder.DropIndex(
                name: "IX_BlogPostTranslations_UpdatedByUserId",
                table: "BlogPostTranslations");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "BlogPostTranslations");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "BlogPostTranslations");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "BlogPostTranslations");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "BlogPostTranslations");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "BlogPostTranslations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BlogPostTranslations");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "BlogPostTranslations");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "BlogPostTranslations");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedByUserId",
                table: "BlogPosts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "BlogPosts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedByUserId",
                table: "BlogPosts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_AspNetUsers_DeletedByUserId",
                table: "BlogPosts",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactMessages_AspNetUsers_CreatedByUserId",
                table: "ContactMessages",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactMessages_AspNetUsers_DeletedByUserId",
                table: "ContactMessages",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactMessages_AspNetUsers_UpdatedByUserId",
                table: "ContactMessages",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaAssets_AspNetUsers_DeletedByUserId",
                table: "MediaAssets",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaAssets_AspNetUsers_UpdatedByUserId",
                table: "MediaAssets",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PageContents_AspNetUsers_CreatedByUserId",
                table: "PageContents",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PageContents_AspNetUsers_DeletedByUserId",
                table: "PageContents",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PageContents_AspNetUsers_UpdatedByUserId",
                table: "PageContents",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PageDefinitions_AspNetUsers_CreatedByUserId",
                table: "PageDefinitions",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PageDefinitions_AspNetUsers_DeletedByUserId",
                table: "PageDefinitions",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PageDefinitions_AspNetUsers_UpdatedByUserId",
                table: "PageDefinitions",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
