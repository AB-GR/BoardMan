using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardMan.Web.Migrations
{
    public partial class AttachmentsAndRoles : Migration
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

            migrationBuilder.AddColumn<Guid>(
                name: "AddedById",
                table: "WorkspaceMembers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "WorkspaceMembers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.AddColumn<Guid>(
                name: "AddedById",
                table: "EmailInvites",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "EmailInvites",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AddedById",
                table: "BoardMembers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "BoardMembers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Description", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { new Guid("da39b930-7150-4a95-b483-767ed2a1be6e"), new DateTime(2022, 3, 19, 8, 20, 6, 754, DateTimeKind.Utc).AddTicks(446), null, "This is a readonly role meant to give view access to users", null, "Read-Only" },
                    { new Guid("db490797-1fbb-4d4a-99c4-d3fe37950fb2"), new DateTime(2022, 3, 19, 8, 20, 6, 754, DateTimeKind.Utc).AddTicks(997), null, "This is an admin role meant for overall access", null, "Admin" },
                    { new Guid("f093fd41-4d47-47af-a814-e199be624237"), new DateTime(2022, 3, 19, 8, 20, 6, 754, DateTimeKind.Utc).AddTicks(958), null, "This is a readwrite role meant to give access to read and write different entities", null, "Read-Write" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceMembers_AddedById",
                table: "WorkspaceMembers",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceMembers_RoleId",
                table: "WorkspaceMembers",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailInvites_AddedById",
                table: "EmailInvites",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmailInvites_RoleId",
                table: "EmailInvites",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_BoardMembers_AddedById",
                table: "BoardMembers",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_BoardMembers_RoleId",
                table: "BoardMembers",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardMembers_AspNetUsers_AddedById",
                table: "BoardMembers",
                column: "AddedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardMembers_Roles_RoleId",
                table: "BoardMembers",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailInvites_AspNetUsers_AddedById",
                table: "EmailInvites",
                column: "AddedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailInvites_Roles_RoleId",
                table: "EmailInvites",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceMembers_AspNetUsers_AddedById",
                table: "WorkspaceMembers",
                column: "AddedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceMembers_Roles_RoleId",
                table: "WorkspaceMembers",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardMembers_AspNetUsers_AddedById",
                table: "BoardMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_BoardMembers_Roles_RoleId",
                table: "BoardMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailInvites_AspNetUsers_AddedById",
                table: "EmailInvites");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailInvites_Roles_RoleId",
                table: "EmailInvites");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceMembers_AspNetUsers_AddedById",
                table: "WorkspaceMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceMembers_Roles_RoleId",
                table: "WorkspaceMembers");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_WorkspaceMembers_AddedById",
                table: "WorkspaceMembers");

            migrationBuilder.DropIndex(
                name: "IX_WorkspaceMembers_RoleId",
                table: "WorkspaceMembers");

            migrationBuilder.DropIndex(
                name: "IX_EmailInvites_AddedById",
                table: "EmailInvites");

            migrationBuilder.DropIndex(
                name: "IX_EmailInvites_RoleId",
                table: "EmailInvites");

            migrationBuilder.DropIndex(
                name: "IX_BoardMembers_AddedById",
                table: "BoardMembers");

            migrationBuilder.DropIndex(
                name: "IX_BoardMembers_RoleId",
                table: "BoardMembers");           

            migrationBuilder.DropColumn(
                name: "AddedById",
                table: "WorkspaceMembers");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "WorkspaceMembers");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "TaskAttachments");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "TaskAttachments");

            migrationBuilder.DropColumn(
                name: "AddedById",
                table: "EmailInvites");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "EmailInvites");

            migrationBuilder.DropColumn(
                name: "AddedById",
                table: "BoardMembers");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "BoardMembers");

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
