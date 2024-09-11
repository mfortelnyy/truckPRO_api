using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace truckPRO_api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLogEntrySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrls",
                table: "LogEntry",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "LogEntry");
        }
    }
}
