using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReCVEServer.Migrations
{
    public partial class addingcveID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cveID",
                table: "CVEs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cveID",
                table: "CVEs");
        }
    }
}
