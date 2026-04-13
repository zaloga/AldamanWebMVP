using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldaman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RouteSegmentRemovedFromPageDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RouteSegment",
                table: "PageDefinitions");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ContactMessages",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RouteSegment",
                table: "PageDefinitions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ContactMessages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048);
        }
    }
}
