using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace truckPRO_api.Migrations
{
    /// <inheritdoc />
    public partial class AddLogEntriesField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FcmDeviceToken",
                table: "User",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "LogEntryId",
                table: "LogEntry",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogEntry_LogEntryId",
                table: "LogEntry",
                column: "LogEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_LogEntry_LogEntry_LogEntryId",
                table: "LogEntry",
                column: "LogEntryId",
                principalTable: "LogEntry",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogEntry_LogEntry_LogEntryId",
                table: "LogEntry");

            migrationBuilder.DropIndex(
                name: "IX_LogEntry_LogEntryId",
                table: "LogEntry");

            migrationBuilder.DropColumn(
                name: "LogEntryId",
                table: "LogEntry");

            migrationBuilder.AlterColumn<string>(
                name: "FcmDeviceToken",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
