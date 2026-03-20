using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieApp.Domain.Entities;

namespace MovieApp.Infrastructure.Data;

public static class DbSeeder
{
    private const int BatchSize = 500;

    /// <param name="contentRootPath">Optional host content root when the JSON is not next to the API assembly.</param>
    public static async Task SeedAsync(
        AppDbContext context,
        ILogger logger,
        bool forceFullReseed,
        string? contentRootPath = null)
    {
        var relative = Path.Combine("Data", "Seed", "moviedata.json");
        var seedFilePath = ResolveSeedFilePath(relative, contentRootPath);

        if (seedFilePath is null)
        {
            logger.LogWarning(
                "Seed file not found at {Relative}. Checked output directory and ContentRoot.",
                relative);
            return;
        }

        var autoDetectRestore = context.ChangeTracker.AutoDetectChangesEnabled;
        context.ChangeTracker.AutoDetectChangesEnabled = false;

        try
        {
            if (forceFullReseed)
            {
                logger.LogInformation("ForceFullReseed: truncating Movies and resetting identity.");
                await context.Database.ExecuteSqlRawAsync(
                    """TRUNCATE TABLE "Movies" RESTART IDENTITY;""");
                context.ChangeTracker.Clear();
            }
            else if (await context.Movies.AnyAsync())
            {
                logger.LogInformation(
                    "Database already has movies; skipping seed. Set Seed:ForceFullReseed to true (Development only) to replace.");
                return;
            }

            var json = await File.ReadAllTextAsync(seedFilePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var rows = JsonSerializer.Deserialize<List<MovieSeedDto>>(json, options);

            if (rows is null || rows.Count == 0)
            {
                logger.LogWarning("Seed file {Path} contained no movies.", seedFilePath);
                return;
            }

            var movies = rows.Select(ToMovie).ToList();

            var inserted = 0;
            for (var i = 0; i < movies.Count; i += BatchSize)
            {
                var batch = movies.Skip(i).Take(BatchSize).ToList();
                await context.Movies.AddRangeAsync(batch);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
                inserted += batch.Count;
                logger.LogInformation("Seeded {Inserted} / {Total} movies.", inserted, movies.Count);
            }

            logger.LogInformation("Seeding finished: {Count} movies from {Path}.", movies.Count, seedFilePath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Seeding failed using file {Path}.", seedFilePath ?? relative);
            throw;
        }
        finally
        {
            context.ChangeTracker.AutoDetectChangesEnabled = autoDetectRestore;
        }
    }

    private static string? ResolveSeedFilePath(string relative, string? contentRootPath)
    {
        var fromBase = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relative));
        if (File.Exists(fromBase))
            return fromBase;

        if (!string.IsNullOrWhiteSpace(contentRootPath))
        {
            var fromContent = Path.GetFullPath(Path.Combine(contentRootPath, relative));
            if (File.Exists(fromContent))
                return fromContent;
        }

        return null;
    }

    private static Movie ToMovie(MovieSeedDto m) => new()
    {
        Title = m.Title?.ToString() ?? string.Empty,
        Year = m.Year,
        Directors = m.Directors ?? string.Empty,
        ReleaseDate = NormalizeReleaseDate(m.ReleaseDate),
        Rating = m.Rating,
        Genres = SplitCommaSeparated(m.Genres),
        ImageUrl = m.ImageUrl ?? string.Empty,
        Plot = m.Plot ?? string.Empty,
        Rank = m.Rank,
        RunningTimeSecs = m.RunningTimeSecs,
        Actors = SplitCommaSeparated(m.Actors)
    };

    /// <summary>Matches <see cref="AppDbContext"/> value conversion (comma-separated, trimmed).</summary>
    private static List<string> SplitCommaSeparated(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return [];

        return raw
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .ToList();
    }

    private static DateTime NormalizeReleaseDate(DateTime d) => d.Kind switch
    {
        DateTimeKind.Utc => d,
        DateTimeKind.Local => d.ToUniversalTime(),
        _ => DateTime.SpecifyKind(d, DateTimeKind.Utc)
    };
}
