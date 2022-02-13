using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardMan.Web.Migrations
{
    public partial class PlansSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Plans",
                columns: new[] { "Id", "Cost", "CreatedAt", "Currency", "DeletedAt", "Description", "ExpireAt", "ModifiedAt", "Name", "PlanType" },
                values: new object[,]
                {
                    { new Guid("13f244b1-ed46-4b80-963e-97a431d2ae5b"), 99m, new DateTime(2022, 2, 13, 3, 32, 57, 162, DateTimeKind.Utc).AddTicks(4226), "USD", null, "This is the standard plan monthly plan", new DateTime(2023, 2, 13, 3, 32, 57, 162, DateTimeKind.Utc).AddTicks(4229), new DateTime(2022, 2, 13, 3, 32, 57, 162, DateTimeKind.Utc).AddTicks(4228), "Standard", 0 },
                    { new Guid("67a528c0-019f-45f1-b2f2-c68d660efedb"), 299m, new DateTime(2022, 2, 13, 3, 32, 57, 162, DateTimeKind.Utc).AddTicks(4274), "USD", null, "This is the premium plan monthly plan", new DateTime(2023, 2, 13, 3, 32, 57, 162, DateTimeKind.Utc).AddTicks(4274), new DateTime(2022, 2, 13, 3, 32, 57, 162, DateTimeKind.Utc).AddTicks(4274), "Premium", 0 },
                    { new Guid("7215dea9-4b77-4c11-9712-fc0d14f2164a"), 3000m, new DateTime(2022, 2, 13, 3, 32, 57, 162, DateTimeKind.Utc).AddTicks(4284), "USD", null, "This is the premium plan annual plan", new DateTime(2023, 2, 13, 3, 32, 57, 162, DateTimeKind.Utc).AddTicks(4285), new DateTime(2022, 2, 13, 3, 32, 57, 162, DateTimeKind.Utc).AddTicks(4285), "Premium", 1 },
                    { new Guid("8793dfb0-ba6c-4f8a-ac7f-0a694b048b5d"), 948m, new DateTime(2022, 2, 13, 3, 32, 57, 162, DateTimeKind.Utc).AddTicks(4262), "USD", null, "This is the standard plan annual plan", new DateTime(2023, 2, 13, 3, 32, 57, 162, DateTimeKind.Utc).AddTicks(4263), new DateTime(2022, 2, 13, 3, 32, 57, 162, DateTimeKind.Utc).AddTicks(4262), "Standard", 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("13f244b1-ed46-4b80-963e-97a431d2ae5b"));

            migrationBuilder.DeleteData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("67a528c0-019f-45f1-b2f2-c68d660efedb"));

            migrationBuilder.DeleteData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("7215dea9-4b77-4c11-9712-fc0d14f2164a"));

            migrationBuilder.DeleteData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("8793dfb0-ba6c-4f8a-ac7f-0a694b048b5d"));
        }
    }
}
