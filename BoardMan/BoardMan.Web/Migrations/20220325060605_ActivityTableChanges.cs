using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardMan.Web.Migrations
{
    public partial class ActivityTableChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewValue",
                table: "ActivityTrackings");

            migrationBuilder.DropColumn(
                name: "PropertyName",
                table: "ActivityTrackings");

            migrationBuilder.RenameColumn(
                name: "OldValue",
                table: "ActivityTrackings",
                newName: "EntityDisplayName");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "ActivityTrackings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "ActivityTrackings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Succeeded",
                table: "ActivityTrackings",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChangedProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newsequentialid()"),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityTrackingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangedProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChangedProperties_ActivityTrackings_ActivityTrackingId",
                        column: x => x.ActivityTrackingId,
                        principalTable: "ActivityTrackings",
                        principalColumn: "Id");
                });            

            migrationBuilder.CreateIndex(
                name: "IX_ChangedProperties_ActivityTrackingId",
                table: "ChangedProperties",
                column: "ActivityTrackingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChangedProperties");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "ActivityTrackings");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "ActivityTrackings");

            migrationBuilder.DropColumn(
                name: "Succeeded",
                table: "ActivityTrackings");

            migrationBuilder.RenameColumn(
                name: "EntityDisplayName",
                table: "ActivityTrackings",
                newName: "OldValue");

            migrationBuilder.AddColumn<string>(
                name: "NewValue",
                table: "ActivityTrackings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PropertyName",
                table: "ActivityTrackings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
