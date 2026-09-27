using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syed.PortfolioPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPDFLinkToArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PDFLink",
                schema: "dbo",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PDFLink",
                schema: "dbo",
                table: "Articles");
        }
    }
}
