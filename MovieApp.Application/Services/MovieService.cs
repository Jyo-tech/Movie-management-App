using MovieApp.Application.DTOs;
using MovieApp.Application.Interfaces;
using MovieApp.Domain.Entities;
using MovieApp.Domain.Interfaces;
using MovieApp.Application.Mapper;

namespace MovieApp.Application.Services;

public class MovieService : IMovieService
{
    private const int MaxSearchPageSize = 100;

    private readonly IMovieRepository _movieRepository;
    private readonly MapToMovieDto _mapper = new MapToMovieDto();

    /// <summary>
    /// Npgsql maps <c>timestamptz</c> to UTC only; JSON/date inputs are usually <see cref="DateTimeKind.Unspecified"/>.
    /// </summary>
    private static DateTime ToPersistableUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
        _mapper = new MapToMovieDto();

    }

    public async Task<IEnumerable<MovieDto>> GetLatestMoviesAsync(int count = 4)
    {
        var movies = await _movieRepository.GetLatestMoviesAsync(count);
        
        return movies.Select(m => _mapper.MapToDto(m));
    }

    public async Task<PagedMoviesDto> SearchMoviesAsync(string? title, string? genre, int? year, int page = 1, int pageSize = 24)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, MaxSearchPageSize);

        var pageResult = await _movieRepository.SearchAsync(title, genre, year, page, pageSize);

        return new PagedMoviesDto
        {
            Items = pageResult.Items.Select(m => _mapper.MapToDto(m)).ToList(),
            TotalCount = pageResult.TotalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<MovieDto> CreateMovieAsync(MovieDto movieDto)
    {
        var movie = new Movie
        {
            Title = movieDto.Title,
            Year = movieDto.Year,
            Directors = movieDto.Directors,
            ReleaseDate = ToPersistableUtc(movieDto.ReleaseDate),
            Rating = movieDto.Rating,
           Genres = movieDto.Genres
            .Select(g => g.Trim())
            .Where(g => !string.IsNullOrWhiteSpace(g))
            .Distinct()
            .ToArray(),
            Actors = movieDto.Actors,
            ImageUrl = movieDto.ImageUrl,
            Plot = movieDto.Plot,
            Rank = movieDto.Rank,
            RunningTimeSecs = movieDto.RunningTimeSecs
        };

        await _movieRepository.AddAsync(movie);
        await _movieRepository.SaveChangesAsync();

        return _mapper.MapToDto(movie);
    }

    public async Task<MovieDto?> GetMovieByIdAsync(int id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        
        if (movie == null)
            return null;

        return _mapper.MapToDto(movie);
    }

    public async Task UpdateMovieAsync(int id, MovieDto movieDto)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie == null)
            throw new KeyNotFoundException($"Movie with id {id} not found.");

        movie.Title = movieDto.Title;
        movie.Year = movieDto.Year;
        movie.Directors = movieDto.Directors;
        movie.ReleaseDate = ToPersistableUtc(movieDto.ReleaseDate);
        movie.Rating = movieDto.Rating;
        movie.Genres = movieDto.Genres
            .Select(g => g.Trim())
            .Where(g => !string.IsNullOrWhiteSpace(g))
            .Distinct()
            .ToArray();
        movie.Actors = movieDto.Actors;
        movie.ImageUrl = movieDto.ImageUrl;
        movie.Plot = movieDto.Plot;
        movie.Rank = movieDto.Rank;
        movie.RunningTimeSecs = movieDto.RunningTimeSecs;

        await _movieRepository.UpdateAsync(movie);
        await _movieRepository.SaveChangesAsync();
    }

    public async Task DeleteMovieAsync(int id)
    {
        await _movieRepository.DeleteAsync(id);
        await _movieRepository.SaveChangesAsync();
    }


}
