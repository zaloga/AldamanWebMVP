using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldaman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteAdjustments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostTranslations_AspNetUsers_DeletedByUserId",
                table: "BlogPostTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentPageTranslations_AspNetUsers_DeletedByUserId",
                table: "ContentPageTranslations");

            migrationBuilder.DropIndex(
                name: "IX_ContentPageTranslations_DeletedByUserId",
                table: "ContentPageTranslations");

            migrationBuilder.DropIndex(
                name: "IX_BlogPostTranslations_DeletedByUserId",
                table: "BlogPostTranslations");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "ContentPageTranslations");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "ContentPageTranslations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ContentPageTranslations");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "BlogPostTranslations");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "BlogPostTranslations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BlogPostTranslations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "ContentPageTranslations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "ContentPageTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ContentPageTranslations",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
                name: "IsDeleted",
                table: "BlogPostTranslations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ContentPageTranslations_DeletedByUserId",
                table: "ContentPageTranslations",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTranslations_DeletedByUserId",
                table: "BlogPostTranslations",
                column: "DeletedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostTranslations_AspNetUsers_DeletedByUserId",
                table: "BlogPostTranslations",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentPageTranslations_AspNetUsers_DeletedByUserId",
                table: "ContentPageTranslations",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
