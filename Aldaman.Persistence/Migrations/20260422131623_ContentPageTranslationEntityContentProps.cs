using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldaman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ContentPageTranslationEntityContentProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BodyDeltaJson",
                table: "ContentPageTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyHtml",
                table: "ContentPageTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlainText",
                table: "ContentPageTranslations",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyDeltaJson",
                table: "ContentPageTranslations");

            migrationBuilder.DropColumn(
                name: "BodyHtml",
                table: "ContentPageTranslations");

            migrationBuilder.DropColumn(
                name: "PlainText",
                table: "ContentPageTranslations");
        }
    }
}
