using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldaman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ShowOnHomePageAndPageOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHomePage",
                table: "PageDefinitions");

            migrationBuilder.AddColumn<int>(
                name: "PageOrder",
                table: "PageDefinitions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnHomePage",
                table: "PageDefinitions",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageOrder",
                table: "PageDefinitions");

            migrationBuilder.DropColumn(
                name: "ShowOnHomePage",
                table: "PageDefinitions");

            migrationBuilder.AddColumn<bool>(
                name: "IsHomePage",
                table: "PageDefinitions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
