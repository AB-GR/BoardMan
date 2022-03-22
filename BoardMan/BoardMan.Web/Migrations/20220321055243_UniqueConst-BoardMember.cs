using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardMan.Web.Migrations
{
    public partial class UniqueConstBoardMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BoardMembers_MemberId",
                table: "BoardMembers");            

            migrationBuilder.AlterColumn<string>(
                name: "EmailAddress",
                table: "EmailInvites",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");            

            migrationBuilder.CreateIndex(
                name: "IX_EmailInvites_EmailAddress_RoleId",
                table: "EmailInvites",
                columns: new[] { "EmailAddress", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoardMembers_MemberId_RoleId",
                table: "BoardMembers",
                columns: new[] { "MemberId", "RoleId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmailInvites_EmailAddress_RoleId",
                table: "EmailInvites");

            migrationBuilder.DropIndex(
                name: "IX_BoardMembers_MemberId_RoleId",
                table: "BoardMembers");           

            migrationBuilder.AlterColumn<string>(
                name: "EmailAddress",
                table: "EmailInvites",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");           

            migrationBuilder.CreateIndex(
                name: "IX_BoardMembers_MemberId",
                table: "BoardMembers",
                column: "MemberId");
        }
    }
}
