using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldaman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToContactMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentPageTranslations_ContentPages_ContentPageId",
                table: "ContentPageTranslations");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "ContactMessages",
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

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_CreatedByUserId",
                table: "ContactMessages",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_DeletedByUserId",
                table: "ContactMessages",
                column: "DeletedByUserId");

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
                name: "FK_ContentPageTranslations_ContentPages_ContentPageId",
                table: "ContentPageTranslations",
                column: "ContentPageId",
                principalTable: "ContentPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_AspNetUsers_CreatedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_AspNetUsers_DeletedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentPageTranslations_ContentPages_ContentPageId",
                table: "ContentPageTranslations");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_CreatedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_DeletedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ContactMessages");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentPageTranslations_ContentPages_ContentPageId",
                table: "ContentPageTranslations",
                column: "ContentPageId",
                principalTable: "ContentPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
