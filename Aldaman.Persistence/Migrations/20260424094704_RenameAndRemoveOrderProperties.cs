using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldaman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameAndRemoveOrderProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderInNavigation",
                table: "ContentPages");

            migrationBuilder.RenameColumn(
                name: "OrderOnHomePage",
                table: "ContentPages",
                newName: "PageOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PageOrder",
                table: "ContentPages",
                newName: "OrderOnHomePage");

            migrationBuilder.AddColumn<int>(
                name: "OrderInNavigation",
                table: "ContentPages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
