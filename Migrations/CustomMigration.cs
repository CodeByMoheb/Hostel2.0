using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Hostel2._0.Migrations
{
    public partial class CustomMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add IsManagerCreated to Students table
            migrationBuilder.AddColumn<bool>(
                name: "IsManagerCreated",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Add RegistrationCode to Students table
            migrationBuilder.AddColumn<string>(
                name: "RegistrationCode",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            // Add CompletedAt to MaintenanceRequests table
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "MaintenanceRequests",
                type: "datetime2",
                nullable: true);

            // Add IssueType to MaintenanceRequests table
            migrationBuilder.AddColumn<string>(
                name: "IssueType",
                table: "MaintenanceRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            // Add Location to MaintenanceRequests table
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "MaintenanceRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            // Add Notes to MaintenanceRequests table
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "MaintenanceRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove Notes from MaintenanceRequests table
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "MaintenanceRequests");

            // Remove Location from MaintenanceRequests table
            migrationBuilder.DropColumn(
                name: "Location",
                table: "MaintenanceRequests");

            // Remove IssueType from MaintenanceRequests table
            migrationBuilder.DropColumn(
                name: "IssueType",
                table: "MaintenanceRequests");

            // Remove CompletedAt from MaintenanceRequests table
            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "MaintenanceRequests");

            // Remove RegistrationCode from Students table
            migrationBuilder.DropColumn(
                name: "RegistrationCode",
                table: "Students");

            // Remove IsManagerCreated from Students table
            migrationBuilder.DropColumn(
                name: "IsManagerCreated",
                table: "Students");
        }
    }
} 