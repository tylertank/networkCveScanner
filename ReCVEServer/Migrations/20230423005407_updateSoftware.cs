using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReCVEServer.Migrations
{
    public partial class updateSoftware : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint("UK_Software", "Software", new string[4]{ "client", "vendor", "application", "version"});
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
