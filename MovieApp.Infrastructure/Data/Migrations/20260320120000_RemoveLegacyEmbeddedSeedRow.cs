using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieApp.Infrastructure.Data.Migrations;

/// <summary>
/// Cleans up the single row previously inserted by an older InitialCreate migration.
/// Fresh databases never get that row; this avoids duplicate key issues when the runtime seeder runs.
/// </summary>
public partial class RemoveLegacyEmbeddedSeedRow : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """DELETE FROM "Movies" WHERE "Id" = 1 AND "Title" = 'Gettysburg';""");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
