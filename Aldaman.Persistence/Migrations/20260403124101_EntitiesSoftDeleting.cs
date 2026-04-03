using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldaman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesSoftDeleting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "PageDefinitions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "PageDefinitions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PageDefinitions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "PageDefinitions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SeoDescription",
                table: "PageContents",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoKeywords",
                table: "PageContents",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "MediaAssets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "MediaAssets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MediaAssets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "MediaAssets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "MediaAssets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "ContactMessages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "ContactMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ContactMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "ContactMessages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "ContactMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "BlogPosts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "BlogPosts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BlogPosts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_PageDefinitions_DeletedByUserId",
                table: "PageDefinitions",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PageDefinitions_UpdatedByUserId",
                table: "PageDefinitions",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_DeletedByUserId",
                table: "MediaAssets",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_UpdatedByUserId",
                table: "MediaAssets",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_DeletedByUserId",
                table: "ContactMessages",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_UpdatedByUserId",
                table: "ContactMessages",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_DeletedByUserId",
                table: "BlogPosts",
                column: "DeletedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_AspNetUsers_DeletedByUserId",
                table: "BlogPosts",
                column: "DeletedByUserId",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_AspNetUsers_DeletedByUserId",
                table: "BlogPosts");

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
                name: "FK_PageDefinitions_AspNetUsers_DeletedByUserId",
                table: "PageDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_PageDefinitions_AspNetUsers_UpdatedByUserId",
                table: "PageDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_PageDefinitions_DeletedByUserId",
                table: "PageDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_PageDefinitions_UpdatedByUserId",
                table: "PageDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_DeletedByUserId",
                table: "MediaAssets");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_UpdatedByUserId",
                table: "MediaAssets");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_DeletedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_UpdatedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_DeletedByUserId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "PageDefinitions");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "PageDefinitions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PageDefinitions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "PageDefinitions");

            migrationBuilder.DropColumn(
                name: "SeoKeywords",
                table: "PageContents");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "MediaAssets");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "MediaAssets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MediaAssets");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "MediaAssets");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "MediaAssets");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BlogPosts");

            migrationBuilder.AlterColumn<string>(
                name: "SeoDescription",
                table: "PageContents",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);
        }
    }
}
