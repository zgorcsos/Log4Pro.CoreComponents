using Microsoft.EntityFrameworkCore.Migrations;

namespace Log4Pro.CoreComponents.Settings.DAL.Migrations.SQLite
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
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UserLevelSettings",
                schema: "settings",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "settings",
                table: "SettingHistories",
                type: "TEXT",
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
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Options",
                schema: "settings",
                table: "Settings",
                type: "TEXT",
                nullable: true);
        }
    }
}
