using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace truckPRO_api.Migrations
{
    /// <inheritdoc />
    public partial class AddParentLogEntryId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogEntry_LogEntry_LogEntryId",
                table: "LogEntry");

            migrationBuilder.DropIndex(
                name: "IX_LogEntry_LogEntryId",
                table: "LogEntry");

            migrationBuilder.RenameColumn(
                name: "LogEntryId",
                table: "LogEntry",
                newName: "ParentLogEntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParentLogEntryId",
                table: "LogEntry",
                newName: "LogEntryId");

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
    }
}
