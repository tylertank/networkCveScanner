using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReCVEServer.Migrations
{
    public partial class softwareCols : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "details",
                table: "Softwares",
                newName: "version");

            migrationBuilder.AddColumn<string>(
                name: "application",
                table: "Softwares",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "vendor",
                table: "Softwares",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "application",
                table: "Softwares");

            migrationBuilder.DropColumn(
                name: "vendor",
                table: "Softwares");

            migrationBuilder.RenameColumn(
                name: "version",
                table: "Softwares",
                newName: "details");
        }
    }
}
