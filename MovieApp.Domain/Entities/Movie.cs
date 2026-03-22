namespace MovieApp.Domain.Entities;

public class Movie
{
    public int Id { get; set; } // Primary Key
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Directors { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; } 
    public double Rating { get; set; }
    
     public string[] Genres { get; set; } = Array.Empty<string>();

    public List<string> Actors { get; set; } = new();

    public string ImageUrl { get; set; } = string.Empty;
    public string Plot { get; set; } = string.Empty;
    public int Rank { get; set; }
    
    // Storing in seconds to match the JSON, but we will format it as HH:mm:ss in the API/Frontend as requested.
    public int RunningTimeSecs { get; set; }


}
