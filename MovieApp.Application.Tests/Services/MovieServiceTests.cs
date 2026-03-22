using MovieApp.Application.DTOs;
using MovieApp.Application.Services;
using MovieApp.Domain.Entities;
using MovieApp.Domain.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace MovieApp.Application.Tests.Services;

[TestFixture]
public class MovieServiceTests
{
    private IMovieRepository _repository = null!;
    private MovieService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IMovieRepository>();
        _sut = new MovieService(_repository);
    }

    private static Movie SampleEntity(int id, string title = "T", int year = 2020) => new()
    {
        Id = id,
        Title = title,
        Year = year,
        Directors = "D",
        ReleaseDate = new DateTime(year, 1, 1),
        Rating = 7,
        Genres = new string [] { "Action" },
        Actors = new List<string> { "A" },
        ImageUrl = "https://example.com/a.jpg",
        Plot = "P",
        Rank = 1,
        RunningTimeSecs = 3600
    };

    private static MovieDto SampleDto(int id = 0) => new()
    {
        Id = id,
        Title = "New",
        Year = 2022,
        Directors = "Dir",
        ReleaseDate = new DateTime(2022, 5, 1),
        Rating = 6.5,
        Genres = new List<string> { "Comedy" },
        Actors = new List<string> { "B" },
        ImageUrl = "https://example.com/b.jpg",
        Plot = "Plot",
        Rank = 3,
        RunningTimeSecs = 1800
    };

    [Test]
    public async Task GetLatestMoviesAsync_ReturnsMappedDtos()
    {
        var movies = new[] { SampleEntity(1, "A"), SampleEntity(2, "B") };
        _repository.GetLatestMoviesAsync(4).Returns(movies);

        var result = (await _sut.GetLatestMoviesAsync(4)).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Title, Is.EqualTo("A"));
        Assert.That(result[1].Id, Is.EqualTo(2));
        await _repository.Received(1).GetLatestMoviesAsync(4);
    }

    [Test]
    public async Task SearchMoviesAsync_ReturnsMappedDtos()
    {
        var movies = new[] { SampleEntity(3, "FindMe") };
        _repository.SearchAsync("x", "g", 1999, 1, 24).Returns(new MovieSearchPage(movies, 42));

        var result = await _sut.SearchMoviesAsync("x", "g", 1999, 1, 24);

        Assert.That(result.Items, Has.Count.EqualTo(1));
        Assert.That(result.Items[0].Title, Is.EqualTo("FindMe"));
        Assert.That(result.TotalCount, Is.EqualTo(42));
        Assert.That(result.Page, Is.EqualTo(1));
        Assert.That(result.PageSize, Is.EqualTo(24));
        await _repository.Received(1).SearchAsync("x", "g", 1999, 1, 24);
    }

    [Test]
    public async Task CreateMovieAsync_AddsSaveChanges_AndReturnsMappedDto_WithGeneratedIdFromEntity()
    {
        var dto = SampleDto(0);
        Movie? captured = null;
        _repository.When(r => r.AddAsync(Arg.Any<Movie>()))
            .Do(ci => captured = ci.Arg<Movie>());

        var returned = await _sut.CreateMovieAsync(dto);

        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.Title, Is.EqualTo("New"));
        Assert.That(captured.Genres, Is.EqualTo(dto.Genres));
        await _repository.Received(1).AddAsync(Arg.Any<Movie>());
        await _repository.Received(1).SaveChangesAsync();
        Assert.That(returned.Title, Is.EqualTo(dto.Title));
        Assert.That(returned.Year, Is.EqualTo(dto.Year));
    }

    [Test]
    public async Task GetMovieByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repository.GetByIdAsync(99).Returns((Movie?)null);

        var result = await _sut.GetMovieByIdAsync(99);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetMovieByIdAsync_ReturnsDto_WhenFound()
    {
        _repository.GetByIdAsync(1).Returns(SampleEntity(1, "One"));

        var result = await _sut.GetMovieByIdAsync(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Title, Is.EqualTo("One"));
    }

    [Test]
    public async Task UpdateMovieAsync_ThrowsKeyNotFound_WhenMissing()
    {
        _repository.GetByIdAsync(7).Returns((Movie?)null);

        try
        {
            await _sut.UpdateMovieAsync(7, SampleDto());
            Assert.Fail("Expected KeyNotFoundException");
        }
        catch (KeyNotFoundException ex)
        {
            Assert.That(ex.Message, Does.Contain("7"));
        }
    }

    [Test]
    public async Task UpdateMovieAsync_UpdatesEntitySaveChanges()
    {
        var existing = SampleEntity(4, "Old", 2000);
        _repository.GetByIdAsync(4).Returns(existing);
        var dto = SampleDto(4);
        dto.Title = "Updated";
        dto.Year = 2005;

        await _sut.UpdateMovieAsync(4, dto);

        Assert.That(existing.Title, Is.EqualTo("Updated"));
        Assert.That(existing.Year, Is.EqualTo(2005));
        await _repository.Received(1).UpdateAsync(existing);
        await _repository.Received(1).SaveChangesAsync();
    }

    [Test]
    public async Task DeleteMovieAsync_CallsRepositoryDeleteAndSave()
    {
        await _sut.DeleteMovieAsync(11);

        await _repository.Received(1).DeleteAsync(11);
        await _repository.Received(1).SaveChangesAsync();
    }
}
