using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Log4Pro.CoreComponents.DIServices.Settings.DAL.Migrations
{
    public partial class InitilaizeDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "settings");

            migrationBuilder.CreateTable(
                name: "SettingHistories",
                schema: "settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Changer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModuleKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InstanceOrUserKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SettingKey = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OperationType = table.Column<int>(type: "int", nullable: false),
                    To = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    From = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                schema: "settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InstanceOrUserKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Key = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Options = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SettingHistories_Changer",
                schema: "settings",
                table: "SettingHistories",
                column: "Changer");

            migrationBuilder.CreateIndex(
                name: "IX_SettingHistories_InstanceOrUserKey",
                schema: "settings",
                table: "SettingHistories",
                column: "InstanceOrUserKey");

            migrationBuilder.CreateIndex(
                name: "IX_SettingHistories_ModuleKey",
                schema: "settings",
                table: "SettingHistories",
                column: "ModuleKey");

            migrationBuilder.CreateIndex(
                name: "IX_SettingHistories_SettingKey",
                schema: "settings",
                table: "SettingHistories",
                column: "SettingKey");

            migrationBuilder.CreateIndex(
                name: "IX_SettingHistories_TimeStamp",
                schema: "settings",
                table: "SettingHistories",
                column: "TimeStamp");

            migrationBuilder.CreateIndex(
                name: "IX_SettingHistories_Version",
                schema: "settings",
                table: "SettingHistories",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_InstanceOrUserKey",
                schema: "settings",
                table: "Settings",
                column: "InstanceOrUserKey");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_Key",
                schema: "settings",
                table: "Settings",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_ModuleKey",
                schema: "settings",
                table: "Settings",
                column: "ModuleKey");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_ModuleKey_InstanceOrUserKey_Key",
                schema: "settings",
                table: "Settings",
                columns: new[] { "ModuleKey", "InstanceOrUserKey", "Key" },
                unique: true,
                filter: "[ModuleKey] IS NOT NULL AND [InstanceOrUserKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_Version",
                schema: "settings",
                table: "Settings",
                column: "Version");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SettingHistories",
                schema: "settings");

            migrationBuilder.DropTable(
                name: "Settings",
                schema: "settings");
        }
    }
}
