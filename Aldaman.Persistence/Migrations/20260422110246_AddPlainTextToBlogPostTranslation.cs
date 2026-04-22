using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldaman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlainTextToBlogPostTranslation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsentToProcessing",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "SearchText",
                table: "BlogPostTranslations");

            migrationBuilder.AddColumn<string>(
                name: "PlainText",
                table: "BlogPostTranslations",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlainText",
                table: "BlogPostTranslations");

            migrationBuilder.AddColumn<bool>(
                name: "ConsentToProcessing",
                table: "ContactMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SearchText",
                table: "BlogPostTranslations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
