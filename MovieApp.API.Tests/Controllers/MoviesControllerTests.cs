using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using MovieApp.API.Controllers;
using MovieApp.Application.DTOs;
using MovieApp.Application.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace MovieApp.API.Tests.Controllers;

[TestFixture]
public class MoviesControllerTests
{
    private IMovieService _movieService = null!;
    private MoviesController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _movieService = Substitute.For<IMovieService>();
        var logger = NullLogger<MoviesController>.Instance;
        _controller = new MoviesController(_movieService, logger);
    }

    private static MovieDto SampleMovie(int id = 1) => new()
    {
        Id = id,
        Title = "Test Movie",
        Year = 2020,
        Directors = "Test Director",
        ReleaseDate = new DateTime(2020, 6, 15),
        Rating = 7.5,
        Genres = new List<string> { "Action" },
        Actors = new List<string> { "Actor One" },
        ImageUrl = "https://example.com/poster.jpg",
        Plot = "A test plot.",
        Rank = 1,
        RunningTimeSecs = 7200
    };

    #region GetLatestMovies

    [Test]
    public async Task GetLatestMovies_ReturnsOk_WithMoviesFromService()
    {
        var movies = new[] { SampleMovie(1), SampleMovie(2) };
        _movieService.GetLatestMoviesAsync(4).Returns(movies);

        var result = await _controller.GetLatestMovies();

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var ok = (OkObjectResult)result.Result!;
        Assert.That(ok.Value, Is.EqualTo(movies));
        await _movieService.Received(1).GetLatestMoviesAsync(4);
    }

    [Test]
    public async Task GetLatestMovies_Returns500_WhenServiceThrows()
    {
        _movieService.GetLatestMoviesAsync(4)
            .Returns(Task.FromException<IEnumerable<MovieDto>>(new InvalidOperationException("db error")));

        var result = await _controller.GetLatestMovies();

        Assert.That(result.Result, Is.TypeOf<ObjectResult>());
        var obj = (ObjectResult)result.Result!;
        Assert.That(obj.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    #endregion

    #region SearchMovies

    [Test]
    public async Task SearchMovies_ReturnsOk_WithMatchingMovies()
    {
        var movies = new[] { SampleMovie(3) };
        var paged = new PagedMoviesDto
        {
            Items = movies.ToList(),
            TotalCount = 1,
            Page = 1,
            PageSize = 24
        };
        _movieService.SearchMoviesAsync("Inception", "Sci-Fi", 2010, 1, 24).Returns(paged);

        var result = await _controller.SearchMovies("Inception", "Sci-Fi", 2010, 1, 24);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var ok = (OkObjectResult)result.Result!;
        Assert.That(ok.Value, Is.EqualTo(paged));
        await _movieService.Received(1).SearchMoviesAsync("Inception", "Sci-Fi", 2010, 1, 24);
    }

    [Test]
    public async Task SearchMovies_Returns500_WhenServiceThrows()
    {
        _movieService.SearchMoviesAsync(Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<int?>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromException<PagedMoviesDto>(new InvalidOperationException()));

        var result = await _controller.SearchMovies(null, null, null, 1, 24);

        Assert.That(result.Result, Is.TypeOf<ObjectResult>());
        Assert.That(((ObjectResult)result.Result!).StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    #endregion

    #region CreateMovie

    [Test]
    public async Task CreateMovie_ReturnsCreated_WhenValid()
    {
        var dto = SampleMovie(0);
        var created = SampleMovie(42);
        _movieService.CreateMovieAsync(dto).Returns(created);

        var result = await _controller.CreateMovie(dto);

        Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
        var createdResult = (CreatedAtActionResult)result.Result!;
        Assert.That(createdResult.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
        Assert.That(createdResult.Value, Is.EqualTo(created));
        Assert.That(createdResult.ActionName, Is.EqualTo(nameof(MoviesController.GetMovieById)));
        await _movieService.Received(1).CreateMovieAsync(dto);
    }

    [Test]
    public async Task CreateMovie_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var dto = SampleMovie();
        _controller.ModelState.AddModelError(nameof(MovieDto.Title), "Required");

        var result = await _controller.CreateMovie(dto);

        Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
        await _movieService.DidNotReceive().CreateMovieAsync(Arg.Any<MovieDto>());
    }

    [Test]
    public async Task CreateMovie_Returns500_WhenServiceThrows()
    {
        var dto = SampleMovie();
        _movieService.CreateMovieAsync(dto)
            .Returns(Task.FromException<MovieDto>(new Exception("fail")));

        var result = await _controller.CreateMovie(dto);

        Assert.That(result.Result, Is.TypeOf<ObjectResult>());
        Assert.That(((ObjectResult)result.Result!).StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    #endregion

    #region GetMovieById

    [Test]
    public async Task GetMovieById_ReturnsOk_WhenFound()
    {
        var movie = SampleMovie(7);
        _movieService.GetMovieByIdAsync(7).Returns(movie);

        var result = await _controller.GetMovieById(7);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        Assert.That(((OkObjectResult)result.Result!).Value, Is.EqualTo(movie));
    }

    [Test]
    public async Task GetMovieById_ReturnsNotFound_WhenMissing()
    {
        _movieService.GetMovieByIdAsync(99).Returns((MovieDto?)null);

        var result = await _controller.GetMovieById(99);

        Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task GetMovieById_Returns500_WhenServiceThrows()
    {
        _movieService.GetMovieByIdAsync(1)
            .Returns(Task.FromException<MovieDto?>(new InvalidOperationException()));

        var result = await _controller.GetMovieById(1);

        Assert.That(result.Result, Is.TypeOf<ObjectResult>());
        Assert.That(((ObjectResult)result.Result!).StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    #endregion

    #region UpdateMovie

    [Test]
    public async Task UpdateMovie_ReturnsNoContent_WhenSuccessful()
    {
        var dto = SampleMovie(5);
        _movieService.GetMovieByIdAsync(5).Returns(dto);

        var result = await _controller.UpdateMovie(5, dto);

        Assert.That(result, Is.TypeOf<NoContentResult>());
        await _movieService.Received(1).UpdateMovieAsync(5, dto);
    }

    [Test]
    public async Task UpdateMovie_ReturnsNotFound_WhenMovieDoesNotExist()
    {
        var dto = SampleMovie(5);
        _movieService.GetMovieByIdAsync(5).Returns((MovieDto?)null);

        var result = await _controller.UpdateMovie(5, dto);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
        await _movieService.DidNotReceive().UpdateMovieAsync(Arg.Any<int>(), Arg.Any<MovieDto>());
    }

    [Test]
    public async Task UpdateMovie_ReturnsBadRequest_WhenModelStateInvalid()
    {
        var dto = SampleMovie(5);
        _movieService.GetMovieByIdAsync(5).Returns(dto);
        _controller.ModelState.AddModelError("Title", "Invalid");

        var result = await _controller.UpdateMovie(5, dto);

        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        await _movieService.DidNotReceive().UpdateMovieAsync(Arg.Any<int>(), Arg.Any<MovieDto>());
    }

    [Test]
    public async Task UpdateMovie_Returns500_WhenServiceThrows()
    {
        var dto = SampleMovie(1);
        _movieService.GetMovieByIdAsync(1).Returns(dto);
        _movieService.UpdateMovieAsync(1, dto).Returns(Task.FromException(new Exception("fail")));

        var result = await _controller.UpdateMovie(1, dto);

        Assert.That(result, Is.TypeOf<ObjectResult>());
        Assert.That(((ObjectResult)result).StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    #endregion

    #region DeleteMovie

    [Test]
    public async Task DeleteMovie_ReturnsNoContent_WhenSuccessful()
    {
        _movieService.GetMovieByIdAsync(3).Returns(SampleMovie(3));

        var result = await _controller.DeleteMovie(3);

        Assert.That(result, Is.TypeOf<NoContentResult>());
        await _movieService.Received(1).DeleteMovieAsync(3);
    }

    [Test]
    public async Task DeleteMovie_ReturnsNotFound_WhenMovieDoesNotExist()
    {
        _movieService.GetMovieByIdAsync(3).Returns((MovieDto?)null);

        var result = await _controller.DeleteMovie(3);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
        await _movieService.DidNotReceive().DeleteMovieAsync(Arg.Any<int>());
    }

    [Test]
    public async Task DeleteMovie_Returns500_WhenServiceThrows()
    {
        _movieService.GetMovieByIdAsync(3).Returns(SampleMovie(3));
        _movieService.DeleteMovieAsync(3).Returns(Task.FromException(new Exception("fail")));

        var result = await _controller.DeleteMovie(3);

        Assert.That(result, Is.TypeOf<ObjectResult>());
        Assert.That(((ObjectResult)result).StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    #endregion
}
