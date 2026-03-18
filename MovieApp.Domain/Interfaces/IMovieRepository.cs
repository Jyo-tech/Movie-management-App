using MovieApp.Domain.Entities;

namespace MovieApp.Domain.Interfaces;

public interface IMovieRepository
{
    Task<Movie?> GetByIdAsync(int id);
    Task<IEnumerable<Movie>> GetAllAsync();
    Task<IEnumerable<Movie>> GetLatestMoviesAsync(int count);
    
    // Updated search to match the new properties (e.g. string for genre since it's dynamic)
    Task<IEnumerable<Movie>> SearchAsync(string? title, string? genre, int? year);
    
    Task AddAsync(Movie movie);
    Task UpdateAsync(Movie movie);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}
