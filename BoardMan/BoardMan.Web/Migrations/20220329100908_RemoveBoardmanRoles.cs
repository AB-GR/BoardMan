using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardMan.Web.Migrations
{
    public partial class RemoveBoardmanRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

			migrationBuilder.DropTable(
				name: "BoardmanRoles");

			migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("0e13590b-566c-4d8d-a39c-0ecfbeaeaa73"), "23e5335a-23ac-4a5c-af83-b2c331241a6a", "WorkspaceAdmin", "WorkspaceAdmin" },
                    { new Guid("19752dc1-eee0-4f6c-8e72-667efe292620"), "c193d4fc-6b8b-4f70-b68f-44346edc5609", "BoardSuperAdmin", "BoardSuperAdmin" },
                    { new Guid("6987c099-cb95-4e07-805f-1f200086d981"), "5787e344-64f9-4f3f-9912-3df6f02fe294", "BoardAdmin", "BoardAdmin" },
                    { new Guid("87d90f66-6b16-453c-85bc-67e910502531"), "db45ce28-7578-4dae-a927-7df216b5a049", "WorkspaceReader", "WorkspaceReader" },
                    { new Guid("940ecb4f-9fe3-47f2-baa4-0bf0b78b17d0"), "0fa88d76-3211-461c-aeba-d486651dde98", "BoardContributor", "BoardContributor" },
                    { new Guid("a393c00f-ff31-4acf-8003-89b18ee96cec"), "e569716e-7f14-452c-8ef5-8cc2aba313a8", "WorkspaceContributor", "WorkspaceContributor" },
                    { new Guid("b25f5ae2-cfab-439d-b3ce-d5834e5e0a7b"), "4917fbc5-4492-4b49-a137-88846547d922", "WorkspaceSuperAdmin", "WorkspaceSuperAdmin" },
                    { new Guid("f58e9f8a-8288-4cbc-8942-b2f401513911"), "13e86009-bfb1-4547-bc81-958d6ca492c1", "ApplicationSuperAdmin", "ApplicationSuperAdmin" },
                    { new Guid("f5ff3262-6a47-4186-b65f-0190c7fc0baf"), "8e87d1c1-62ad-4c95-a01a-651de6455611", "BoardReader", "BoardReader" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_BoardMembers_AspNetRoles_RoleId",
                table: "BoardMembers",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailInvites_AspNetRoles_RoleId",
                table: "EmailInvites",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceMembers_AspNetRoles_RoleId",
                table: "WorkspaceMembers",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardMembers_AspNetRoles_RoleId",
                table: "BoardMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailInvites_AspNetRoles_RoleId",
                table: "EmailInvites");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceMembers_AspNetRoles_RoleId",
                table: "WorkspaceMembers");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("0e13590b-566c-4d8d-a39c-0ecfbeaeaa73"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("19752dc1-eee0-4f6c-8e72-667efe292620"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("6987c099-cb95-4e07-805f-1f200086d981"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("87d90f66-6b16-453c-85bc-67e910502531"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("940ecb4f-9fe3-47f2-baa4-0bf0b78b17d0"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a393c00f-ff31-4acf-8003-89b18ee96cec"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("b25f5ae2-cfab-439d-b3ce-d5834e5e0a7b"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("f58e9f8a-8288-4cbc-8942-b2f401513911"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("f5ff3262-6a47-4186-b65f-0190c7fc0baf"));           

            migrationBuilder.CreateTable(
                name: "BoardmanRoles",
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
    }
}
