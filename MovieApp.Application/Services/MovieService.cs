using MovieApp.Application.DTOs;
using MovieApp.Application.Interfaces;
using MovieApp.Domain.Entities;
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
        
        return movies.Select(m => MapToDto(m));
    }

    public async Task<IEnumerable<MovieDto>> SearchMoviesAsync(string? title, string? genre, int? year)
    {
        var movies = await _movieRepository.SearchAsync(title, genre, year);
        
        return movies.Select(m => MapToDto(m));
    }

    public async Task<MovieDto> CreateMovieAsync(MovieDto movieDto)
    {
        var movie = new Movie
        {
            Title = movieDto.Title,
            Year = movieDto.Year,
            Directors = movieDto.Directors,
            ReleaseDate = movieDto.ReleaseDate,
            Rating = movieDto.Rating,
            Genres = movieDto.Genres,
            Actors = movieDto.Actors,
            ImageUrl = movieDto.ImageUrl,
            Plot = movieDto.Plot,
            Rank = movieDto.Rank,
            RunningTimeSecs = movieDto.RunningTimeSecs
        };

        await _movieRepository.AddAsync(movie);
        await _movieRepository.SaveChangesAsync();

        return MapToDto(movie);
    }

    private MovieDto MapToDto(Movie m)
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
