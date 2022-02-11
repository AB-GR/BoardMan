using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardMan.Web.Migrations
{
    public partial class StartedAtTypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StarteAt",
                table: "Subscriptions",
                newName: "StartedAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartedAt",
                table: "Subscriptions",
                newName: "StarteAt");
        }
    }
}
