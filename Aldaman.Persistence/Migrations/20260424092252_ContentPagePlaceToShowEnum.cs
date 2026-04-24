using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldaman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ContentPagePlaceToShowEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShowOnHomePage",
                table: "ContentPages",
                newName: "PlaceToShow");

            migrationBuilder.AlterColumn<int>(
                name: "PlaceToShow",
                table: "ContentPages",
                type: "int",
                nullable: false,
                oldType: "bit",
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlaceToShow",
                table: "ContentPages",
                newName: "ShowOnHomePage");

            migrationBuilder.AlterColumn<bool>(
                name: "ShowOnHomePage",
                table: "ContentPages",
                type: "bit",
                nullable: false,
                oldType: "int",
                defaultValue: true);
        }
    }
}
