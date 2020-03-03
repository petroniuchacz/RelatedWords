using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RelatedWordsAPI.Migrations
{
    public partial class AddedFilters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FilterId",
                table: "Word",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Filter",
                columns: table => new
                {
                    FilterId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(nullable: false),
                    EditRevisionNumber = table.Column<int>(nullable: false, defaultValue: 0),
                    Name = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filter", x => x.FilterId);
                    table.ForeignKey(
                        name: "FK_Filter_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FilterWord",
                columns: table => new
                {
                    FilterWordId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FilterId = table.Column<int>(nullable: false),
                    WordContent = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterWord", x => x.FilterWordId);
                    table.ForeignKey(
                        name: "FK_FilterWord_Filter_FilterId",
                        column: x => x.FilterId,
                        principalTable: "Filter",
                        principalColumn: "FilterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFilter",
                columns: table => new
                {
                    FilterId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFilter", x => new { x.FilterId, x.ProjectId });
                    table.ForeignKey(
                        name: "FK_ProjectFilter_Filter_FilterId",
                        column: x => x.FilterId,
                        principalTable: "Filter",
                        principalColumn: "FilterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectFilter_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Word_FilterId",
                table: "Word",
                column: "FilterId");

            migrationBuilder.CreateIndex(
                name: "IX_Filter_UserId",
                table: "Filter",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterWord_FilterId",
                table: "FilterWord",
                column: "FilterId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFilter_ProjectId",
                table: "ProjectFilter",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Word_Filter_FilterId",
                table: "Word",
                column: "FilterId",
                principalTable: "Filter",
                principalColumn: "FilterId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Word_Filter_FilterId",
                table: "Word");

            migrationBuilder.DropTable(
                name: "FilterWord");

            migrationBuilder.DropTable(
                name: "ProjectFilter");

            migrationBuilder.DropTable(
                name: "Filter");

            migrationBuilder.DropIndex(
                name: "IX_Word_FilterId",
                table: "Word");

            migrationBuilder.DropColumn(
                name: "FilterId",
                table: "Word");
        }
    }
}
