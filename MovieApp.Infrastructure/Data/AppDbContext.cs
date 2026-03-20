using Microsoft.EntityFrameworkCore;
using MovieApp.Domain.Entities;
using System.Text.Json;
using System.IO;

namespace MovieApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }


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
           modelBuilder.Entity<Movie>()
        .Property(m => m.Genres)
        .HasColumnType("text[]");
        
            entity.Property(e => e.Actors)
                .HasConversion(
                    v => string.Join(", ", v),
                    v => v.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList()
                );
        });

      
    }
}

internal class MovieSeedDto
{
    public object? Title { get; set; }
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
