using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldaman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BaseEntityUnification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaAssets_AspNetUsers_UploadedByUserId",
                table: "MediaAssets");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_UploadedByUserId",
                table: "MediaAssets");

            migrationBuilder.DropColumn(
                name: "UploadedByUserId",
                table: "MediaAssets");

            migrationBuilder.RenameColumn(
                name: "UploadedAtUtc",
                table: "MediaAssets",
                newName: "CreatedAtUtc");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "PageDefinitions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "PageContents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "PageContents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "PageContents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PageContents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PageContents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "PageContents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "MediaAssets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ContactMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ContactMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "BlogPosts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_PageDefinitions_CreatedByUserId",
                table: "PageDefinitions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PageContents_CreatedByUserId",
                table: "PageContents",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PageContents_DeletedByUserId",
                table: "PageContents",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PageContents_UpdatedByUserId",
                table: "PageContents",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_CreatedByUserId",
                table: "MediaAssets",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_CreatedByUserId",
                table: "ContactMessages",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactMessages_AspNetUsers_CreatedByUserId",
                table: "ContactMessages",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaAssets_AspNetUsers_CreatedByUserId",
                table: "MediaAssets",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_AspNetUsers_CreatedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaAssets_AspNetUsers_CreatedByUserId",
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

            migrationBuilder.DropIndex(
                name: "IX_PageDefinitions_CreatedByUserId",
                table: "PageDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_PageContents_CreatedByUserId",
                table: "PageContents");

            migrationBuilder.DropIndex(
                name: "IX_PageContents_DeletedByUserId",
                table: "PageContents");

            migrationBuilder.DropIndex(
                name: "IX_PageContents_UpdatedByUserId",
                table: "PageContents");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_CreatedByUserId",
                table: "MediaAssets");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_CreatedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "PageDefinitions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "PageContents");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "PageContents");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "PageContents");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PageContents");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PageContents");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "PageContents");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "MediaAssets");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "BlogPosts");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "MediaAssets",
                newName: "UploadedAtUtc");

            migrationBuilder.AddColumn<Guid>(
                name: "UploadedByUserId",
                table: "MediaAssets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_UploadedByUserId",
                table: "MediaAssets",
                column: "UploadedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaAssets_AspNetUsers_UploadedByUserId",
                table: "MediaAssets",
                column: "UploadedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
