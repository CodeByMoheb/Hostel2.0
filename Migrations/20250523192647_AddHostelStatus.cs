using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hostel2._0.Migrations
{
    /// <inheritdoc />
    public partial class AddHostelStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Hostels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Hostels");
        }
    }
}
