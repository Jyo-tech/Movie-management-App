using Microsoft.EntityFrameworkCore;
using MovieApp.Domain.Entities;
using MovieApp.Domain.Interfaces;
using MovieApp.Infrastructure.Data;

namespace MovieApp.Infrastructure.Repositories;

public class MovieRepository : IMovieRepository
{
    public const int MaxSearchPageSize = 100;

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

    public async Task<MovieSearchPage> SearchAsync(string? title, string? genre, int? year, int page, int pageSize)
    {
        // page = Math.Max(1, page);
        // pageSize = Math.Clamp(pageSize, 1, MaxSearchPageSize);

        var query = _context.Movies.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(title))
        {
            var t = title.Trim();
            query = query.Where(m => m.Title.ToLower() == t.ToLower());
        }

        if (year.HasValue)
            query = query.Where(m => m.Year == year.Value);

        if (!string.IsNullOrWhiteSpace(genre))
        {
            var g = genre.Trim();
            query = query.Where(m => m.Genres.Any(x => x.ToLower() == g.ToLower()));
        }

        query = query.OrderByDescending(m => m.ReleaseDate).ThenBy(m => m.Id);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new MovieSearchPage(items, totalCount);
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
