using MovieApp.Domain.Entities;

namespace MovieApp.Domain.Interfaces;

public interface IMovieRepository
{
    Task<Movie?> GetByIdAsync(int id);
    Task<IEnumerable<Movie>> GetAllAsync();
    Task<IEnumerable<Movie>> GetLatestMoviesAsync(int count);
    
    /// <param name="page">1-based page index.</param>
    /// <param name="pageSize">Rows per page (clamped by implementation).</param>
    Task<MovieSearchPage> SearchAsync(string? title, string? genre, int? year, int page, int pageSize);

    Task AddAsync(Movie movie);
    Task UpdateAsync(Movie movie);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}

/// <summary>One page of <see cref="Movie"/> rows plus the total matching count for pagination.</summary>
public sealed record MovieSearchPage(IReadOnlyList<Movie> Items, int TotalCount);
