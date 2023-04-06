using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReCVEServer.Migrations
{
    public partial class addedstatustablerenamedtoclient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vulnerabilities");

            migrationBuilder.DropTable(
                name: "Computers");

            migrationBuilder.AddColumn<string>(
                name: "application",
                table: "CVEs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "vendor",
                table: "CVEs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "version",
                table: "CVEs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OSVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    processStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cpu = table.Column<float>(type: "real", nullable: false),
                    memory = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Softwares",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    clientID = table.Column<int>(type: "int", nullable: false),
                    CVEID = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    details = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Softwares", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Softwares_Clients_clientID",
                        column: x => x.clientID,
                        principalTable: "Clients",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Softwares_CVEs_CVEID",
                        column: x => x.CVEID,
                        principalTable: "CVEs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Softwares_clientID",
                table: "Softwares",
                column: "clientID");

            migrationBuilder.CreateIndex(
                name: "IX_Softwares_CVEID",
                table: "Softwares",
                column: "CVEID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Softwares");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropColumn(
                name: "application",
                table: "CVEs");

            migrationBuilder.DropColumn(
                name: "vendor",
                table: "CVEs");

            migrationBuilder.DropColumn(
                name: "version",
                table: "CVEs");

            migrationBuilder.CreateTable(
                name: "Computers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Computers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Vulnerabilities",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    computerID = table.Column<int>(type: "int", nullable: false),
                    CVEID = table.Column<int>(type: "int", nullable: false),
                    details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vulnerabilities", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Vulnerabilities_Computers_computerID",
                        column: x => x.computerID,
                        principalTable: "Computers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vulnerabilities_CVEs_CVEID",
                        column: x => x.CVEID,
                        principalTable: "CVEs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vulnerabilities_computerID",
                table: "Vulnerabilities",
                column: "computerID");

            migrationBuilder.CreateIndex(
                name: "IX_Vulnerabilities_CVEID",
                table: "Vulnerabilities",
                column: "CVEID");
        }
    }
}
