using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Log4Pro.CoreComponents.OperationMessageCenter.DAL.Migrations
{
    public partial class InitializeDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "omcenter");

            migrationBuilder.CreateTable(
                name: "OperationMessages",
                schema: "omcenter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Module = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Instance = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OtherFilter = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MessageCategory = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Handled = table.Column<bool>(type: "bit", nullable: false),
                    HandledTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HandledBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Thread = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdditionalMessageDatas",
                schema: "omcenter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperationMessageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalMessageDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdditionalMessageDatas_OperationMessages_OperationMessageId",
                        column: x => x.OperationMessageId,
                        principalSchema: "omcenter",
                        principalTable: "OperationMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalMessageDatas_DataKey",
                schema: "omcenter",
                table: "AdditionalMessageDatas",
                column: "DataKey");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalMessageDatas_OperationMessageId",
                schema: "omcenter",
                table: "AdditionalMessageDatas",
                column: "OperationMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationMessages_Handled",
                schema: "omcenter",
                table: "OperationMessages",
                column: "Handled");

            migrationBuilder.CreateIndex(
                name: "IX_OperationMessages_HandledTimeStamp",
                schema: "omcenter",
                table: "OperationMessages",
                column: "HandledTimeStamp");

            migrationBuilder.CreateIndex(
                name: "IX_OperationMessages_Instance",
                schema: "omcenter",
                table: "OperationMessages",
                column: "Instance");

            migrationBuilder.CreateIndex(
                name: "IX_OperationMessages_MessageCategory",
                schema: "omcenter",
                table: "OperationMessages",
                column: "MessageCategory");

            migrationBuilder.CreateIndex(
                name: "IX_OperationMessages_Module",
                schema: "omcenter",
                table: "OperationMessages",
                column: "Module");

            migrationBuilder.CreateIndex(
                name: "IX_OperationMessages_OtherFilter",
                schema: "omcenter",
                table: "OperationMessages",
                column: "OtherFilter");

            migrationBuilder.CreateIndex(
                name: "IX_OperationMessages_Thread",
                schema: "omcenter",
                table: "OperationMessages",
                column: "Thread");

            migrationBuilder.CreateIndex(
                name: "IX_OperationMessages_TimeStamp",
                schema: "omcenter",
                table: "OperationMessages",
                column: "TimeStamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdditionalMessageDatas",
                schema: "omcenter");

            migrationBuilder.DropTable(
                name: "OperationMessages",
                schema: "omcenter");
        }
    }
}
