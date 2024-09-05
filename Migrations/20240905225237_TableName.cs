using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace truckPRO_api.Migrations
{
    /// <inheritdoc />
    public partial class TableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogEntries_User_UserId",
                table: "LogEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LogEntries",
                table: "LogEntries");

            migrationBuilder.RenameTable(
                name: "LogEntries",
                newName: "LogEntry");

            migrationBuilder.RenameIndex(
                name: "IX_LogEntries_UserId",
                table: "LogEntry",
                newName: "IX_LogEntry_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LogEntry",
                table: "LogEntry",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LogEntry_User_UserId",
                table: "LogEntry",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogEntry_User_UserId",
                table: "LogEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LogEntry",
                table: "LogEntry");

            migrationBuilder.RenameTable(
                name: "LogEntry",
                newName: "LogEntries");

            migrationBuilder.RenameIndex(
                name: "IX_LogEntry_UserId",
                table: "LogEntries",
                newName: "IX_LogEntries_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LogEntries",
                table: "LogEntries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LogEntries_User_UserId",
                table: "LogEntries",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
