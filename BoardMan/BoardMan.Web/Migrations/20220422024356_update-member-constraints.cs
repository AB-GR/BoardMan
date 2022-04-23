using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardMan.Web.Migrations
{
    public partial class updatememberconstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkspaceMembers_WorkspaceId",
                table: "WorkspaceMembers");

            migrationBuilder.DropIndex(
                name: "IX_EmailInvites_EmailAddress_RoleId",
                table: "EmailInvites");

            migrationBuilder.DropIndex(
                name: "IX_BoardMembers_BoardId",
                table: "BoardMembers");

            migrationBuilder.DropIndex(
                name: "IX_BoardMembers_MemberId_RoleId",
                table: "BoardMembers");

            migrationBuilder.AlterColumn<string>(
                name: "EntityUrn",
                table: "EmailInvites",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateIndex(
                name: "UK_WorkspaceMember_WorkspaceId_MemberId_DeletedAt",
                table: "WorkspaceMembers",
                columns: new[] { "WorkspaceId", "MemberId" },
                unique: true,
                filter: "[DeletedAt] IS NULL");

            migrationBuilder.CreateIndex(
                name: "UK_EmailInvite_EntityUrn_EmailAddress_DeletedAt",
                table: "EmailInvites",
                columns: new[] { "EntityUrn", "EmailAddress" },
                unique: true,
                filter: "[DeletedAt] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BoardMembers_MemberId",
                table: "BoardMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "UK_BoardMember_BoardId_MemberId_DeletedAt",
                table: "BoardMembers",
                columns: new[] { "BoardId", "MemberId" },
                unique: true,
                filter: "[DeletedAt] IS NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UK_WorkspaceMember_WorkspaceId_MemberId_DeletedAt",
                table: "WorkspaceMembers");

            migrationBuilder.DropIndex(
                name: "UK_EmailInvite_EntityUrn_EmailAddress_DeletedAt",
                table: "EmailInvites");

            migrationBuilder.DropIndex(
                name: "IX_BoardMembers_MemberId",
                table: "BoardMembers");

            migrationBuilder.DropIndex(
                name: "UK_BoardMember_BoardId_MemberId_DeletedAt",
                table: "BoardMembers");

            migrationBuilder.AlterColumn<string>(
                name: "EntityUrn",
                table: "EmailInvites",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceMembers_WorkspaceId",
                table: "WorkspaceMembers",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailInvites_EmailAddress_RoleId",
                table: "EmailInvites",
                columns: new[] { "EmailAddress", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoardMembers_BoardId",
                table: "BoardMembers",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_BoardMembers_MemberId_RoleId",
                table: "BoardMembers",
                columns: new[] { "MemberId", "RoleId" },
                unique: true);
        }
    }
}
