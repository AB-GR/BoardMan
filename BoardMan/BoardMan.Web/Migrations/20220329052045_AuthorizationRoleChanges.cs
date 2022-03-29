using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardMan.Web.Migrations
{
    public partial class AuthorizationRoleChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardMembers_Roles_RoleId",
                table: "BoardMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailInvites_Roles_RoleId",
                table: "EmailInvites");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceMembers_Roles_RoleId",
                table: "WorkspaceMembers");

            migrationBuilder.DropTable(
                name: "Roles");           

            migrationBuilder.CreateTable(
                name: "BoardmanRoles",
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
                    table.PrimaryKey("PK_BoardmanRoles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BoardmanRoles",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Description", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { new Guid("041ed186-1c4c-425c-b460-41044ef1939c"), new DateTime(2022, 3, 29, 5, 20, 44, 447, DateTimeKind.Utc).AddTicks(354), null, "Application level global or super admin has all api access including internal api", null, "ApplicationSuperAdmin" },
                    { new Guid("08a9f389-7a90-4206-bc34-bbb54c9fa82c"), new DateTime(2022, 3, 29, 5, 20, 44, 447, DateTimeKind.Utc).AddTicks(371), null, "Workspace level global or super admin has all workspace api access", null, "WorkspaceSuperAdmin" },
                    { new Guid("20040c26-699e-4862-89f7-1ce7a02228ec"), new DateTime(2022, 3, 29, 5, 20, 44, 447, DateTimeKind.Utc).AddTicks(382), null, "Workspace level admin", null, "WorkspaceAdmin" },
                    { new Guid("31da2e0d-3357-424f-b6a0-b6d959df4723"), new DateTime(2022, 3, 29, 5, 20, 44, 447, DateTimeKind.Utc).AddTicks(406), null, "Workspace level contributor", null, "WorkspaceContributor" },
                    { new Guid("33ddc6ae-d3ae-45ed-b8d5-db7735daa4a3"), new DateTime(2022, 3, 29, 5, 20, 44, 447, DateTimeKind.Utc).AddTicks(442), null, "Board level admin", null, "BoardAdmin" },
                    { new Guid("5184d295-4796-49a9-b6b6-678462121bad"), new DateTime(2022, 3, 29, 5, 20, 44, 447, DateTimeKind.Utc).AddTicks(453), null, "Board level contributor", null, "BoardContributor" },
                    { new Guid("8ed8dd1c-3ea5-4df5-be92-7a0a61ce133b"), new DateTime(2022, 3, 29, 5, 20, 44, 447, DateTimeKind.Utc).AddTicks(417), null, "Workspace level reader", null, "WorkspaceReader" },
                    { new Guid("a9912388-ac13-4ea4-afd2-59076d40e1d1"), new DateTime(2022, 3, 29, 5, 20, 44, 447, DateTimeKind.Utc).AddTicks(463), null, "Board level reader", null, "BoardReader" },
                    { new Guid("df090650-169d-414a-96cc-7a6a0cd5c2dd"), new DateTime(2022, 3, 29, 5, 20, 44, 447, DateTimeKind.Utc).AddTicks(432), null, "Board level super admin", null, "BoardSuperAdmin" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_BoardMembers_BoardmanRoles_RoleId",
                table: "BoardMembers",
                column: "RoleId",
                principalTable: "BoardmanRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailInvites_BoardmanRoles_RoleId",
                table: "EmailInvites",
                column: "RoleId",
                principalTable: "BoardmanRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceMembers_BoardmanRoles_RoleId",
                table: "WorkspaceMembers",
                column: "RoleId",
                principalTable: "BoardmanRoles",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardMembers_BoardmanRoles_RoleId",
                table: "BoardMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailInvites_BoardmanRoles_RoleId",
                table: "EmailInvites");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceMembers_BoardmanRoles_RoleId",
                table: "WorkspaceMembers");

            migrationBuilder.DropTable(name: "BoardmanRoles");            

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
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
                    { new Guid("26f1e366-6d41-4ccc-a436-c0601786112b"), new DateTime(2022, 3, 25, 6, 6, 4, 369, DateTimeKind.Utc).AddTicks(7305), null, "This is an admin role meant for overall access", null, "Admin" },
                    { new Guid("47f97124-c8fa-45a4-a304-d5638f16b973"), new DateTime(2022, 3, 25, 6, 6, 4, 369, DateTimeKind.Utc).AddTicks(7283), null, "This is a readonly role meant to give view access to users", null, "Read-Only" },
                    { new Guid("f28f00a9-5f84-45a6-857d-b9c60bf71d06"), new DateTime(2022, 3, 25, 6, 6, 4, 369, DateTimeKind.Utc).AddTicks(7295), null, "This is a readwrite role meant to give access to read and write different entities", null, "Read-Write" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_BoardMembers_Roles_RoleId",
                table: "BoardMembers",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailInvites_Roles_RoleId",
                table: "EmailInvites",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceMembers_Roles_RoleId",
                table: "WorkspaceMembers",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");
        }
    }
}
