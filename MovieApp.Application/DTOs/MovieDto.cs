using System.ComponentModel.DataAnnotations;

namespace MovieApp.Application.DTOs;

public class MovieDto
{
    public int Id { get; set; }

  
    public string Title { get; set; } = string.Empty;

    public int Year { get; set; }

    public string Directors { get; set; } = string.Empty;

    public DateTime ReleaseDate { get; set; }

    public double Rating { get; set; }

    public List<string> Genres { get; set; } = new();

    public List<string> Actors { get; set; } = new();

    public string ImageUrl { get; set; } = string.Empty;

    public string Plot { get; set; } = string.Empty;

    public int Rank { get; set; }

    public int RunningTimeSecs { get; set; }
}
