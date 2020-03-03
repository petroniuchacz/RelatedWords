using Microsoft.EntityFrameworkCore.Migrations;

namespace RelatedWordsAPI.Migrations
{
    public partial class ChangesInFilter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Word_Filter_FilterId",
                table: "Word");

            migrationBuilder.DropIndex(
                name: "IX_Word_FilterId",
                table: "Word");

            migrationBuilder.DropColumn(
                name: "FilterId",
                table: "Word");

            migrationBuilder.AddColumn<int>(
                name: "filterType",
                table: "ProjectFilter",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "filterType",
                table: "ProjectFilter");

            migrationBuilder.AddColumn<int>(
                name: "FilterId",
                table: "Word",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Word_FilterId",
                table: "Word",
                column: "FilterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Word_Filter_FilterId",
                table: "Word",
                column: "FilterId",
                principalTable: "Filter",
                principalColumn: "FilterId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
