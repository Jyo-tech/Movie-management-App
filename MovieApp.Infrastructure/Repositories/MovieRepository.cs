using Microsoft.EntityFrameworkCore;
using MovieApp.Domain.Entities;
using MovieApp.Domain.Interfaces;
using MovieApp.Infrastructure.Data;

namespace MovieApp.Infrastructure.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly AppDbContext _context;

    public MovieRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Movie?> GetByIdAsync(int id)
    {
        return await _context.Movies.FindAsync(id);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        return await _context.Movies.ToListAsync();
    }

    public async Task<IEnumerable<Movie>> GetLatestMoviesAsync(int count)
    {
        return await _context.Movies
            .OrderByDescending(m => m.ReleaseDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Movie>> SearchAsync(string? title, string? genre, int? year)
    {
        var query = _context.Movies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(m => m.Title.ToLower().Contains(title.ToLower()));
        }

        if (year.HasValue)
        {
            query = query.Where(m => m.Year == year.Value);
        }

        
        var results = await query.ToListAsync();

        if (!string.IsNullOrWhiteSpace(genre))
        {
            results = results.Where(m => m.Genres.Any(g => g.Contains(genre, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        return results;
    }

    public async Task AddAsync(Movie movie)
    {
        await _context.Movies.AddAsync(movie);
    }

    public async Task UpdateAsync(Movie movie)
    {
        _context.Movies.Update(movie);
        await Task.CompletedTask; 
    }

    public async Task DeleteAsync(int id)
    {
        var movie = await GetByIdAsync(id);
        if (movie != null)
        {
            _context.Movies.Remove(movie);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
