using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldaman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Media_AltTextDefault_TitleDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "MediaAssets",
                newName: "TitleDefault");

            migrationBuilder.RenameColumn(
                name: "AltText",
                table: "MediaAssets",
                newName: "AltTextDefault");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TitleDefault",
                table: "MediaAssets",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "AltTextDefault",
                table: "MediaAssets",
                newName: "AltText");
        }
    }
}
