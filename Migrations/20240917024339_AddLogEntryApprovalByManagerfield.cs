using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace truckPRO_api.Migrations
{
    /// <inheritdoc />
    public partial class AddLogEntryApprovalByManagerfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApprovedByManager",
                table: "LogEntry",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApprovedByManager",
                table: "LogEntry");
        }
    }
}
