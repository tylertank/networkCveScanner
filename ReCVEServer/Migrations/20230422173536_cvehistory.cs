using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReCVEServer.Migrations
{
    public partial class cvehistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "online",
                table: "Clients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    totalCount = table.Column<int>(type: "int", nullable: false),
                    cveScore = table.Column<double>(type: "float", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    highCount = table.Column<int>(type: "int", nullable: true),
                    lowCount = table.Column<int>(type: "int", nullable: true),
                    mediumCount = table.Column<int>(type: "int", nullable: true),
                    criticalCount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "History");

            migrationBuilder.DropColumn(
                name: "online",
                table: "Clients");
        }
    }
}
