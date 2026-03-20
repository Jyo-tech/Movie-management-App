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

    // GET: api/movies/search?title=&genre=&year=&page=1&pageSize=24
    [HttpGet("search")]
    public async Task<ActionResult<PagedMoviesDto>> SearchMovies(
        [FromQuery] string? title,
        [FromQuery] string? genre,
        [FromQuery] int? year,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 24)
    {
        try
        {
            _logger.LogInformation(
                "Searching movies: title={Title}, genre={Genre}, year={Year}, page={Page}, pageSize={PageSize}.",
                title, genre, year, page, pageSize);
            var result = await _movieService.SearchMoviesAsync(title, genre, year, page, pageSize);
            _logger.LogInformation(
                "Search returned {PageCount} movies (total matching: {Total}).",
                result.Items.Count,
                result.TotalCount);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while searching movies.");
            return StatusCode(500, "Internal server error");
        }
    }

    // POST: api/movies
    [HttpPost]
    public async Task<ActionResult<MovieDto>> CreateMovie([FromBody] MovieDto movieDto)
    {
        try
        {
            _logger.LogInformation("Creating new movie with title: {Title}.", movieDto.Title);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for movie creation.");
                return BadRequest(ModelState);
            }

            var createdMovie = await _movieService.CreateMovieAsync(movieDto);
            _logger.LogInformation("Successfully created movie with id: {MovieId}.", createdMovie.Id);
            return CreatedAtAction(nameof(GetMovieById), new { id = createdMovie.Id }, createdMovie);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating a new movie.");
            return StatusCode(500, "Internal server error");
        }
    }

    // GET: api/movies/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<MovieDto>> GetMovieById(int id)
    {
        try
        {
            _logger.LogInformation("Fetching movie with id: {MovieId}.", id);
            var movie = await _movieService.GetMovieByIdAsync(id);

            if (movie == null)
            {
                _logger.LogWarning("Movie with id: {MovieId} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Successfully fetched movie with id: {MovieId}.", id);
            return Ok(movie);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching movie with id: {MovieId}.", id);
            return StatusCode(500, "Internal server error");
        }
    }

    // PUT: api/movies/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieDto movieDto)
    {
        try
        {
            _logger.LogInformation("Updating movie with id: {MovieId}.", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for movie update.");
                return BadRequest(ModelState);
            }

            var existingMovie = await _movieService.GetMovieByIdAsync(id);
            if (existingMovie == null)
            {
                _logger.LogWarning("Movie with id: {MovieId} not found for update.", id);
                return NotFound();
            }

            await _movieService.UpdateMovieAsync(id, movieDto);
            _logger.LogInformation("Successfully updated movie with id: {MovieId}.", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating movie with id: {MovieId}.", id);
            return StatusCode(500, "Internal server error");
        }
    }

    // DELETE: api/movies/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        try
        {
            _logger.LogInformation("Deleting movie with id: {MovieId}.", id);

            var existingMovie = await _movieService.GetMovieByIdAsync(id);
            if (existingMovie == null)
            {
                _logger.LogWarning("Movie with id: {MovieId} not found for deletion.", id);
                return NotFound();
            }

            await _movieService.DeleteMovieAsync(id);
            _logger.LogInformation("Successfully deleted movie with id: {MovieId}.", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting movie with id: {MovieId}.", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
