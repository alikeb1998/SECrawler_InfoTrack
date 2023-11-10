using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SECrawler.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "SE");

            migrationBuilder.CreateTable(
                name: "Engines",
                schema: "SE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaseUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SearchUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expression = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Engines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchResults",
                schema: "SE",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KeyWords = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rank = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchResults", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "SE",
                table: "Engines",
                columns: new[] { "Id", "BaseUrl", "Expression", "Name", "SearchUrl" },
                values: new object[,]
                {
                    { 1, "http://www.google.co.uk", "gMi0 kCrYT(.+?)sa=U&ved=", "Google", "search?q=#query#&num=#pageSize#" },
                    { 2, "http://www.bing.com", "((<cite>)(.+?)(</cite>))", "Bing", "search?q=#query#" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Engines",
                schema: "SE");

            migrationBuilder.DropTable(
                name: "SearchResults",
                schema: "SE");
        }
    }
}
