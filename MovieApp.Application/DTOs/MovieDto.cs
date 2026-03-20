using System.ComponentModel.DataAnnotations;

namespace MovieApp.Application.DTOs;

public class MovieDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
    public int Year { get; set; }

    [Required(ErrorMessage = "Directors are required.")]
    public string Directors { get; set; } = string.Empty;

    [Required(ErrorMessage = "Release date is required.")]
    public DateTime ReleaseDate { get; set; }

    [Range(0.0, 10.0, ErrorMessage = "Rating must be between 0 and 10.")]
    public double Rating { get; set; }

    [MinLength(1, ErrorMessage = "At least one genre is required.")]
    public List<string> Genres { get; set; } = new();

    [MinLength(1, ErrorMessage = "At least one actor is required.")]
    public List<string> Actors { get; set; } = new();

    [Url(ErrorMessage = "Image URL must be a valid URL.")]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Plot cannot exceed 1000 characters.")]
    public string Plot { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Rank must be greater than 0.")]
    public int Rank { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Running time must be greater than 0.")]
    public int RunningTimeSecs { get; set; }
}
