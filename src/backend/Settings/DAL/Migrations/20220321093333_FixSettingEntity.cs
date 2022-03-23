using Microsoft.EntityFrameworkCore.Migrations;

namespace Log4Pro.CoreComponents.Settings.DAL.Migrations
{
    public partial class FixSettingEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultValue",
                schema: "settings",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Options",
                schema: "settings",
                table: "Settings");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "settings",
                table: "Settings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UserLevelSettings",
                schema: "settings",
                table: "Settings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "settings",
                table: "SettingHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Settings_UserLevelSettings",
                schema: "settings",
                table: "Settings",
                column: "UserLevelSettings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Settings_UserLevelSettings",
                schema: "settings",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "settings",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "UserLevelSettings",
                schema: "settings",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "settings",
                table: "SettingHistories");

            migrationBuilder.AddColumn<string>(
                name: "DefaultValue",
                schema: "settings",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Options",
                schema: "settings",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
