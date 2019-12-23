using Microsoft.EntityFrameworkCore.Migrations;

namespace RelatedWordsAPI.Migrations
{
    public partial class ProjectName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Project",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Project");
        }
    }
}
