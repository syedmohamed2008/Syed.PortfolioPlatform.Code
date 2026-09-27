using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syed.PortfolioPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddYouTubeLinkToArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YouTubeLink",
                schema: "dbo",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YouTubeLink",
                schema: "dbo",
                table: "Articles");
        }
    }
}
