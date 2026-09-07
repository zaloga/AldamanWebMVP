using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aldaman.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesConfigurationsConsolidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_AspNetUsers_CreatedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_AspNetUsers_DeletedByUserId",
                table: "ContactMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_StyleSettings_AspNetUsers_CreatedByUserId",
                table: "StyleSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_StyleSettings_AspNetUsers_DeletedByUserId",
                table: "StyleSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_StyleSettings_AspNetUsers_UpdatedByUserId",
                table: "StyleSettings");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "StyleSettings",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "MediaAssets",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Contents",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "ContentGroups",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "ContactMessages",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactMessages_AspNetUsers_CreatedByUserId",
                table: "ContactMessages",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactMessages_AspNetUsers_DeletedByUserId",
                table: "ContactMessages",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StyleSettings_AspNetUsers_CreatedByUserId",
                table: "StyleSettings",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StyleSettings_AspNetUsers_DeletedByUserId",
                table: "StyleSettings",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StyleSettings_AspNetUsers_UpdatedByUserId",
                table: "StyleSettings",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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
                name: "FK_StyleSettings_AspNetUsers_CreatedByUserId",
                table: "StyleSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_StyleSettings_AspNetUsers_DeletedByUserId",
                table: "StyleSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_StyleSettings_AspNetUsers_UpdatedByUserId",
                table: "StyleSettings");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "StyleSettings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "MediaAssets",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Contents",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "ContentGroups",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "ContactMessages",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

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
                name: "FK_StyleSettings_AspNetUsers_CreatedByUserId",
                table: "StyleSettings",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StyleSettings_AspNetUsers_DeletedByUserId",
                table: "StyleSettings",
                column: "DeletedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StyleSettings_AspNetUsers_UpdatedByUserId",
                table: "StyleSettings",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
