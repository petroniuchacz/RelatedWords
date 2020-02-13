using Microsoft.EntityFrameworkCore.Migrations;

namespace RelatedWordsAPI.Migrations
{
    public partial class ProjectRevisionNumbersCorrected : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EditRevisonNumber",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ProcessingRevisonNumber",
                table: "Project");

            migrationBuilder.AddColumn<int>(
                name: "EditRevisionNumber",
                table: "Project",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcessingRevisionNumber",
                table: "Project",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EditRevisionNumber",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ProcessingRevisionNumber",
                table: "Project");

            migrationBuilder.AddColumn<int>(
                name: "EditRevisonNumber",
                table: "Project",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcessingRevisonNumber",
                table: "Project",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
