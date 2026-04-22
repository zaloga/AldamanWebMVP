using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldaman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameContentPageOrderProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PageOrder",
                table: "ContentPages",
                newName: "OrderOnHomePage");

            migrationBuilder.RenameColumn(
                name: "DefaultSortOrder",
                table: "ContentPages",
                newName: "OrderInNavigation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderOnHomePage",
                table: "ContentPages",
                newName: "PageOrder");

            migrationBuilder.RenameColumn(
                name: "OrderInNavigation",
                table: "ContentPages",
                newName: "DefaultSortOrder");
        }
    }
}
