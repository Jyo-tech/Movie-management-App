using MovieApp.Application.DTOs;

namespace MovieApp.Application.Interfaces;

public interface IMovieService
{
    Task<IEnumerable<MovieDto>> GetLatestMoviesAsync(int count = 4);

    Task<PagedMoviesDto> SearchMoviesAsync(string? title, string? genre, int? year, int page = 1, int pageSize = 24);

    Task<MovieDto> CreateMovieAsync(MovieDto movieDto);

    Task<MovieDto?> GetMovieByIdAsync(int id);

    Task UpdateMovieAsync(int id, MovieDto movieDto);

    Task DeleteMovieAsync(int id);
}
