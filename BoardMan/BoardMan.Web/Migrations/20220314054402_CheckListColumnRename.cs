using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardMan.Web.Migrations
{
    public partial class CheckListColumnRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskChecklists_AspNetUsers_CommentedById",
                table: "TaskChecklists");
           
            migrationBuilder.RenameColumn(
                name: "CommentedById",
                table: "TaskChecklists",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_TaskChecklists_CommentedById",
                table: "TaskChecklists",
                newName: "IX_TaskChecklists_CreatedById");           

            migrationBuilder.AddForeignKey(
                name: "FK_TaskChecklists_AspNetUsers_CreatedById",
                table: "TaskChecklists",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskChecklists_AspNetUsers_CreatedById",
                table: "TaskChecklists");            

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "TaskChecklists",
                newName: "CommentedById");

            migrationBuilder.RenameIndex(
                name: "IX_TaskChecklists_CreatedById",
                table: "TaskChecklists",
                newName: "IX_TaskChecklists_CommentedById");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskChecklists_AspNetUsers_CommentedById",
                table: "TaskChecklists",
                column: "CommentedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
