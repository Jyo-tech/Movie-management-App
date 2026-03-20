using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGenresArray : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           migrationBuilder.Sql(
    @"ALTER TABLE ""Movies"" 
      ALTER COLUMN ""Genres"" TYPE text[]
      USING string_to_array(replace(""Genres"", ' ', ''), ',');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           migrationBuilder.Sql(
    @"ALTER TABLE ""Movies"" 
      ALTER COLUMN ""Genres"" TYPE text
      USING array_to_string(""Genres"", ',');");
        }
    }
}
