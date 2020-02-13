using Microsoft.EntityFrameworkCore.Migrations;

namespace RelatedWordsAPI.Migrations
{
    public partial class ProjectProcessingRevisionNumbersCorrected : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EditPagesRevisionNumber",
                table: "Project",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcessedPagesRevisionNumber",
                table: "Project",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EditPagesRevisionNumber",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ProcessedPagesRevisionNumber",
                table: "Project");
        }
    }
}
