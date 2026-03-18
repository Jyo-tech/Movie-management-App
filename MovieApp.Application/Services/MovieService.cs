using MovieApp.Application.DTOs;
using MovieApp.Application.Interfaces;
using MovieApp.Domain.Interfaces;

namespace MovieApp.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<IEnumerable<MovieDto>> GetLatestMoviesAsync(int count = 4)
    {
        var movies = await _movieRepository.GetLatestMoviesAsync(count);
        
        // Manual mapping from Domain Entity to Application DTO
        return movies.Select(m => new MovieDto
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
        });
    }

    public async Task<IEnumerable<MovieDto>> SearchMoviesAsync(string? title, string? genre, int? year)
    {
        var movies = await _movieRepository.SearchAsync(title, genre, year);
        
        // Manual mapping from Domain Entity to Application DTO
        return movies.Select(m => new MovieDto
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
        });
    }
}
