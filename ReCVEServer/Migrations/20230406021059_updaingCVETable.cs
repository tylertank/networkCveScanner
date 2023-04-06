using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReCVEServer.Migrations
{
    public partial class updaingCVETable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IPAddress",
                table: "CVEs");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CVEs",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "EnrollmentDate",
                table: "CVEs",
                newName: "published");

            migrationBuilder.AddColumn<double>(
                name: "baseScore",
                table: "CVEs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "baseScore",
                table: "CVEs");

            migrationBuilder.RenameColumn(
                name: "published",
                table: "CVEs",
                newName: "EnrollmentDate");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "CVEs",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                table: "CVEs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
