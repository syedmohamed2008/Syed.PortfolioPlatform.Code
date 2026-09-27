using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syed.PortfolioPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                schema: "dbo",
                table: "Articles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CreatedOn",
                schema: "dbo",
                table: "Articles",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_IsPublished_IsDeleted",
                schema: "dbo",
                table: "Articles",
                columns: new[] { "IsPublished", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Articles_CreatedOn",
                schema: "dbo",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_IsPublished_IsDeleted",
                schema: "dbo",
                table: "Articles");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                schema: "dbo",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }
    }
}
