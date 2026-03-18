using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MovieApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Directors = table.Column<string>(type: "text", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    Genres = table.Column<string>(type: "text", nullable: false),
                    Actors = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    Plot = table.Column<string>(type: "text", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    RunningTimeSecs = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "Actors", "Directors", "Genres", "ImageUrl", "Plot", "Rank", "Rating", "ReleaseDate", "RunningTimeSecs", "Title", "Year" },
                values: new object[] { 1, "Tom Berenger, Martin Sheen, Stephen Lang", "Ronald F. Maxwell", "Drama, History, War", "https://ia.media-imdb.com/images/M/MV5BMTQwMjE3NDg4NV5BMl5BanBnXkFtZTcwMDI3MTE2Mw@@._V1_SX400_.jpg", "In 1863, the Northern and Southern forces fight at Gettysburg in the decisive battle of the American Civil War.", 3545, 7.5, new DateTime(1993, 10, 8, 0, 0, 0, 0, DateTimeKind.Utc), 16260, "Gettysburg", 1993 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
