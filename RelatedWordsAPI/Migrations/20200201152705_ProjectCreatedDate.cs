using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RelatedWordsAPI.Migrations
{
    public partial class ProjectCreatedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Page_Project_ProjectId",
                table: "Page");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Project",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Page",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Page_Project_ProjectId",
                table: "Page",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Page_Project_ProjectId",
                table: "Page");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Project");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Page",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Page_Project_ProjectId",
                table: "Page",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
