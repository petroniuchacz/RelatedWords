using Microsoft.EntityFrameworkCore.Migrations;

namespace RelatedWordsAPI.Migrations
{
    public partial class ProjectRevisionNumbers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EditRevisonNumber",
                table: "Project",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcessingRevisonNumber",
                table: "Project",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EditRevisonNumber",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ProcessingRevisonNumber",
                table: "Project");
        }
    }
}
