using BoardMan.Web.Auth;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Security.Cryptography;

namespace BoardMan.Web.Data
{
	public class MetadataManager
	{
        

        public static void InitSeedData(MigrationBuilder migrationBuilder)
		{
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                     { Roles.WorkspaceReaderId, "2978145a-bf1b-427a-bbd9-72d7db6c2ec0", Roles.WorkspaceReader, Roles.WorkspaceReader },
                     { Roles.WorkspaceContributorId, "baacbc8c-0b6d-47e0-b63c-342a4b1bbf17", Roles.WorkspaceContributor, Roles.WorkspaceContributor },
                     { Roles.WorkspaceAdminId, "332c0698-0c38-458a-bfc3-4406c1a1fe01", Roles.WorkspaceAdmin, Roles.WorkspaceAdmin },
                     { Roles.WorkspaceSuperAdminId, "6d7e9ecd-2015-4428-9e90-c6fa7a41f740", Roles.WorkspaceSuperAdmin, Roles.WorkspaceSuperAdmin },
                     { Roles.BoardReaderId, "5a674c00-edd6-4f00-af59-15f1ee9b2422", Roles.BoardReader, Roles.BoardReader },
                     { Roles.BoardContributorId, "8f7aabab-6c05-41d6-82f2-e5096d74a9df", Roles.BoardContributor, Roles.BoardContributor },
                     { Roles.BoardAdminId, "d45eade3-7c12-4034-afc9-8bd176a8f6ed", Roles.BoardAdmin, Roles.BoardAdmin },
                     { Roles.BoardSuperAdminId, "0df98363-0912-409f-8fb0-2aae5482682d", Roles.BoardSuperAdmin, Roles.BoardSuperAdmin },
                     { Roles.ApplicationSuperAdminId, "cb3100af-a0ba-4c87-9c3d-fce7b8e2ebb5", Roles.ApplicationSuperAdmin, Roles.ApplicationSuperAdmin }
                });

            migrationBuilder.InsertData(
                table: "Plans",
                columns: new[] { "Id", "BoardLimit", "Cost", "CreatedAt", "Currency", "DeletedAt", "Description", "ExpireAt", "ModifiedAt", "Name", "PlanType" },
                values: new object[,]
                {
                    { Plans.FreePlanId, 1, 0m, new DateTime(2022, 4, 13, 5, 8, 35, 589, DateTimeKind.Utc).AddTicks(4939), "USD", null, "This is the free plan", new DateTime(2023, 4, 13, 5, 8, 35, 589, DateTimeKind.Utc).AddTicks(4945), null, Plans.FreePlan, 1 },
                    { Plans.StandardMonthlyId, 5, 99m, new DateTime(2022, 4, 13, 5, 8, 35, 589, DateTimeKind.Utc).AddTicks(4980), "USD", null, "This is the standard monthly plan", new DateTime(2023, 4, 13, 5, 8, 35, 589, DateTimeKind.Utc).AddTicks(4980), null, Plans.StandardMonthly, 0 },
                    { Plans.StandardAnnualId, 5, 948m, new DateTime(2022, 4, 13, 5, 8, 35, 589, DateTimeKind.Utc).AddTicks(4992), "USD", null, "This is the standard annual plan", new DateTime(2023, 4, 13, 5, 8, 35, 589, DateTimeKind.Utc).AddTicks(4992), null, Plans.StandardAnnual, 1 },
                    { Plans.PremiumMonthlyId, null, 299m, new DateTime(2022, 4, 13, 5, 8, 35, 589, DateTimeKind.Utc).AddTicks(5002), "USD", null, "This is the premium monthly plan", new DateTime(2023, 4, 13, 5, 8, 35, 589, DateTimeKind.Utc).AddTicks(5003), null, Plans.PremiumMonthly, 0 },
                    { Plans.PremiumAnnualId, null, 3000m, new DateTime(2022, 4, 13, 5, 8, 35, 589, DateTimeKind.Utc).AddTicks(5013), "USD", null, "This is the premium annual plan", new DateTime(2023, 4, 13, 5, 8, 35, 589, DateTimeKind.Utc).AddTicks(5014), null, Plans.PremiumAnnual, 1 }
                });

            migrationBuilder.InsertData(
               table: "AspNetUsers",
               columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
               values: new object[] { Users.ApplicationSuperAdminId, 0, "20b09599-8808-48d9-82bf-0f990458e6d5", Users.ApplicationSuperAdminEmail, true, "Admin", "User", false, null, Users.ApplicationSuperAdminEmail, Users.ApplicationSuperAdminEmail, HashPassword("SuperAdmin@874#"), null, false, "f54f8a84-0390-49ef-8e84-07f66d86e91c", false, Users.ApplicationSuperAdminEmail });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { Roles.ApplicationSuperAdminId, Users.ApplicationSuperAdminId });
        }

        public static void RemoveSeedData(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: Roles.WorkspaceReaderId);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: Roles.WorkspaceContributorId);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: Roles.WorkspaceAdminId);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: Roles.WorkspaceSuperAdminId);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: Roles.BoardReaderId);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: Roles.BoardContributorId);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: Roles.BoardAdminId);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: Roles.BoardSuperAdminId);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: Roles.ApplicationSuperAdminId);

            migrationBuilder.DeleteData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: Plans.FreePlanId);

            migrationBuilder.DeleteData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: Plans.StandardMonthlyId);

            migrationBuilder.DeleteData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: Plans.StandardAnnualId);

            migrationBuilder.DeleteData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: Plans.PremiumMonthlyId);

            migrationBuilder.DeleteData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: Plans.PremiumAnnualId);

            migrationBuilder.DeleteData(
               table: "AspNetUserRoles",
               keyColumns: new[] { "RoleId", "UserId" },
               keyValues: new object[] { Roles.ApplicationSuperAdminId, Users.ApplicationSuperAdminId });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: Users.ApplicationSuperAdminId);
        }

        private static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            using (var bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

    }
}
