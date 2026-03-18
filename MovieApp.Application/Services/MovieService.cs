using MovieApp.Application.DTOs;
using MovieApp.Application.Interfaces;
using MovieApp.Domain.Entities;
using MovieApp.Domain.Interfaces;
using MovieApp.Application.Mapper;

namespace MovieApp.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly MapToMovieDto _mapper = new MapToMovieDto();

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

    public async Task<IEnumerable<MovieDto>> SearchMoviesAsync(string? title, string? genre, int? year)
    {
        var movies = await _movieRepository.SearchAsync(title, genre, year);
        
        return movies.Select(m => _mapper.MapToDto(m));
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
        movie.ReleaseDate = movieDto.ReleaseDate;
        movie.Rating = movieDto.Rating;
        movie.Genres = movieDto.Genres;
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
