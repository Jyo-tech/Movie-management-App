using Microsoft.EntityFrameworkCore;
using MovieApp.Domain.Entities;
using System.Text.Json;
using System.IO;

namespace MovieApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        

        // Configure the Movie entity mapping
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Directors).IsRequired();
            
            // Convert List<string> to comma-separated strings for database storage
            entity.Property(e => e.Genres)
                .HasConversion(
                    v => string.Join(", ", v),
                    v => v.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            entity.Property(e => e.Actors)
                .HasConversion(
                    v => string.Join(", ", v),
                    v => v.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList()
                );
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Define the path to the seed data file.
        // Assuming moviedata.json will be copied to the output directory or lives in the root.
        var seedFilePath = Path.Combine(AppContext.BaseDirectory, "Data", "Seed", "moviedata.json");
        
        if (File.Exists(seedFilePath))
        {
            try
            {
                var jsonData = File.ReadAllText(seedFilePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                
                // We use a temporary class to match the exact JSON property names
                var jsonMovies = JsonSerializer.Deserialize<List<MovieSeedDto>>(jsonData, options);
                
                if (jsonMovies != null && jsonMovies.Any())
                {
                    var idCounter = 1;
                    var moviesToSeed = jsonMovies.Select(m => new Movie
                    {
                        Id = idCounter++, // Assign explicit IDs for seeding
                        Title = m.Title ?? string.Empty,
                        Year = m.Year,
                        Directors = m.Directors ?? string.Empty,
                        ReleaseDate = m.ReleaseDate,
                        Rating = m.Rating,
                        Genres = string.IsNullOrWhiteSpace(m.Genres) ? new List<string>() : m.Genres.Split(", ", StringSplitOptions.TrimEntries).ToList(),
                        ImageUrl = m.ImageUrl ?? string.Empty,
                        Plot = m.Plot ?? string.Empty,
                        Rank = m.Rank,
                        RunningTimeSecs = m.RunningTimeSecs,
                        Actors = string.IsNullOrWhiteSpace(m.Actors) ? new List<string>() : m.Actors.Split(", ", StringSplitOptions.TrimEntries).ToList()
                    }).ToArray();

                    modelBuilder.Entity<Movie>().HasData(moviesToSeed);
                }
            }
            catch (Exception ex)
            {
                // In a real application, you'd log this exception
                Console.WriteLine($"Error seeding data: {ex.Message}");
            }
        }
    }
}

// Internal class specifically for mapping the JSON payload which uses spaces in property names
internal class MovieSeedDto
{
    public string? Title { get; set; }
    public int Year { get; set; }
    public string? Directors { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("Release Date")]
    public DateTime ReleaseDate { get; set; }
    
    public double Rating { get; set; }
    public string? Genres { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("Image URL")]
    public string? ImageUrl { get; set; }
    
    public string? Plot { get; set; }
    public int Rank { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("Running Time (secs)")]
    public int RunningTimeSecs { get; set; }
    
    public string? Actors { get; set; }
}
