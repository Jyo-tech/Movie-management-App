using MovieApp.Application.DTOs;

namespace MovieApp.Application.Interfaces;

public interface IMovieService
{
    Task<IEnumerable<MovieDto>> GetLatestMoviesAsync(int count = 4);

    Task<IEnumerable<MovieDto>> SearchMoviesAsync(string? title, string? genre, int? year);

    Task<MovieDto> CreateMovieAsync(MovieDto movieDto);
}
