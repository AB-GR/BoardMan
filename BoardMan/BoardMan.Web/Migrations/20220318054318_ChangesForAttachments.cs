using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardMan.Web.Migrations
{
    public partial class ChangesForAttachments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "TaskAttachments");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "TaskAttachments",
                newName: "TrustedFileName");

            migrationBuilder.AlterColumn<string>(
                name: "FileUri",
                table: "TaskAttachments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "TaskAttachments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Size",
                table: "TaskAttachments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {           

            migrationBuilder.DropColumn(
                name: "Note",
                table: "TaskAttachments");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "TaskAttachments");

            migrationBuilder.RenameColumn(
                name: "TrustedFileName",
                table: "TaskAttachments",
                newName: "FileName");

            migrationBuilder.AlterColumn<string>(
                name: "FileUri",
                table: "TaskAttachments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "TaskAttachments",
                type: "int",
                maxLength: 25,
                nullable: false,
                defaultValue: 0);
        }
    }
}
