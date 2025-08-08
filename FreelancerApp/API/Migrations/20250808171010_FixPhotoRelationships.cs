using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class FixPhotoRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PortfolioItems_Photos_PhotoId1",
                table: "PortfolioItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Photos_PhotoId1",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_PhotoId1",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_PortfolioItems_PhotoId1",
                table: "PortfolioItems");

            migrationBuilder.DropColumn(
                name: "PhotoId1",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PhotoId1",
                table: "PortfolioItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PhotoId1",
                table: "Projects",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PhotoId1",
                table: "PortfolioItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_PhotoId1",
                table: "Projects",
                column: "PhotoId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioItems_PhotoId1",
                table: "PortfolioItems",
                column: "PhotoId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PortfolioItems_Photos_PhotoId1",
                table: "PortfolioItems",
                column: "PhotoId1",
                principalTable: "Photos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Photos_PhotoId1",
                table: "Projects",
                column: "PhotoId1",
                principalTable: "Photos",
                principalColumn: "Id");
        }
    }
}
