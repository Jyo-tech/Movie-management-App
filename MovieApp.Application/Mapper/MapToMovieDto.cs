using MovieApp.Application.DTOs;
using MovieApp.Domain.Entities;
namespace MovieApp.Application.Mapper
{
    public class MapToMovieDto
    {
     public MovieDto MapToDto(Movie m)
    {
        return new MovieDto
        {
            Id = m.Id,
            Title = m.Title,
            Year = m.Year,
            Directors = m.Directors,
            ReleaseDate = m.ReleaseDate,
            Rating = m.Rating,
            Genres = m.Genres,
            Actors = m.Actors,
            ImageUrl = m.ImageUrl,
            Plot = m.Plot,
            Rank = m.Rank,
            RunningTimeSecs = m.RunningTimeSecs
        };
    }
    }
}