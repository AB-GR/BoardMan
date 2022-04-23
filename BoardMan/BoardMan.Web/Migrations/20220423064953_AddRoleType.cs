using BoardMan.Web.Auth;
using BoardMan.Web.Data;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardMan.Web.Migrations
{
    public partial class AddRoleType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleType",
                table: "AspNetRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData("AspNetRoles", "Id", Roles.ApplicationSuperAdminId, "RoleType", (int)RoleType.ApplicationRole);

            migrationBuilder.UpdateData("AspNetRoles", "Id", Roles.WorkspaceSuperAdminId, "RoleType", (int)RoleType.WorkSpaceRole);
            migrationBuilder.UpdateData("AspNetRoles", "Id", Roles.WorkspaceAdminId, "RoleType", (int)RoleType.WorkSpaceRole);
            migrationBuilder.UpdateData("AspNetRoles", "Id", Roles.WorkspaceContributorId, "RoleType", (int)RoleType.WorkSpaceRole);
            migrationBuilder.UpdateData("AspNetRoles", "Id", Roles.WorkspaceReaderId, "RoleType", (int)RoleType.WorkSpaceRole);

            migrationBuilder.UpdateData("AspNetRoles", "Id", Roles.BoardSuperAdminId, "RoleType", (int)RoleType.BoardRole);
            migrationBuilder.UpdateData("AspNetRoles", "Id", Roles.BoardAdminId, "RoleType", (int)RoleType.BoardRole);
            migrationBuilder.UpdateData("AspNetRoles", "Id", Roles.BoardContributorId, "RoleType", (int)RoleType.BoardRole);
            migrationBuilder.UpdateData("AspNetRoles", "Id", Roles.BoardReaderId, "RoleType", (int)RoleType.BoardRole);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleType",
                table: "AspNetRoles");
        }
    }
}
