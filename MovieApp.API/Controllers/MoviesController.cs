using Microsoft.AspNetCore.Mvc;
using MovieApp.Application.DTOs;
using MovieApp.Application.Interfaces;

namespace MovieApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(IMovieService movieService, ILogger<MoviesController> logger)
    {
        _movieService = movieService;
        _logger = logger;
    }

    // GET: api/movies/latest
    [HttpGet("latest")]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetLatestMovies()
    {
        try
        {
            _logger.LogInformation("Fetching latest 4 movies.");
            // Requirement: Home page should display latest 4 movies
            var movies = await _movieService.GetLatestMoviesAsync(4);
            _logger.LogInformation("Successfully fetched {Count} movies.", movies.Count());
            return Ok(movies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching latest movies.");
            return StatusCode(500, "Internal server error");
        }
    }

    // GET: api/movies/search?title={title}&genre={genre}&year={year}
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<MovieDto>>> SearchMovies([FromQuery] string? title, [FromQuery] string? genre, [FromQuery] int? year)
    {
        try
        {
            _logger.LogInformation("Searching movies with criteria: title={Title}, genre={Genre}, year={Year}.", title, genre, year);
            var movies = await _movieService.SearchMoviesAsync(title, genre, year);
            _logger.LogInformation("Successfully found {Count} movies matching the criteria.", movies.Count());
            return Ok(movies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while searching movies.");
            return StatusCode(500, "Internal server error");
        }
    }
}
