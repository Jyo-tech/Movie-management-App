using Microsoft.EntityFrameworkCore;
using MovieApp.Domain.Entities;
using MovieApp.Infrastructure.Data;
using MovieApp.Infrastructure.Repositories;
using NUnit.Framework;

namespace MovieApp.Infrastructure.Tests.Repositories;

[TestFixture]
public class MovieRepositoryTests
{
    // SearchAsync uses PostgreSQL ILIKE; exercise it against a real DB (e.g. integration tests), not EF InMemory.

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var ctx = new AppDbContext(options);
        ctx.Database.EnsureCreated();
        // Model seed (JSON) may populate rows; tests expect a clean store.
        ctx.Movies.RemoveRange(ctx.Movies);
        ctx.SaveChanges();
        return ctx;
    }

    private static Movie NewMovie(string title, int year, DateTime release, List<string>? genres = null) => new()
    {
        Title = title,
        Year = year,
        Directors = "Director",
        ReleaseDate = release,
        Rating = 7,
        Genres = genres?.ToArray() ?? Array.Empty<string>(),
        Actors = new List<string> { "Actor" },
        ImageUrl = "https://example.com/p.jpg",
        Plot = "Plot",
        Rank = 1,
        RunningTimeSecs = 3600
    };

    [Test]
    public async Task GetByIdAsync_ReturnsMovie_WhenExists()
    {
        await using var ctx = CreateContext();
        var movie = NewMovie("Alpha", 2020, new DateTime(2020, 1, 1));
        ctx.Movies.Add(movie);
        await ctx.SaveChangesAsync();
        var id = movie.Id;
        var sut = new MovieRepository(ctx);

        var found = await sut.GetByIdAsync(id);

        Assert.That(found, Is.Not.Null);
        Assert.That(found!.Title, Is.EqualTo("Alpha"));
    }

    [Test]
    public async Task GetLatestMoviesAsync_OrdersByReleaseDateDescending_TakeCount()
    {
        await using var ctx = CreateContext();
        ctx.Movies.AddRange(
            NewMovie("Old", 2018, new DateTime(2018, 1, 1)),
            NewMovie("Newest", 2022, new DateTime(2022, 6, 1)),
            NewMovie("Mid", 2020, new DateTime(2020, 1, 1)));
        await ctx.SaveChangesAsync();
        var sut = new MovieRepository(ctx);

        var latest = (await sut.GetLatestMoviesAsync(2)).ToList();

        Assert.That(latest, Has.Count.EqualTo(2));
        Assert.That(latest[0].Title, Is.EqualTo("Newest"));
        Assert.That(latest[1].Title, Is.EqualTo("Mid"));
    }

    [Test]
    public async Task AddAndSaveChanges_PersistsMovie()
    {
        await using var ctx = CreateContext();
        var sut = new MovieRepository(ctx);
        var movie = NewMovie("Fresh", 2023, new DateTime(2023, 1, 1));

        await sut.AddAsync(movie);
        await sut.SaveChangesAsync();

        Assert.That(movie.Id, Is.Not.EqualTo(0));
        Assert.That(await ctx.Movies.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task UpdateAsync_PersistsChanges()
    {
        await using var ctx = CreateContext();
        var movie = NewMovie("Orig", 2019, new DateTime(2019, 1, 1));
        ctx.Movies.Add(movie);
        await ctx.SaveChangesAsync();
        var sut = new MovieRepository(ctx);

        movie.Title = "Renamed";
        await sut.UpdateAsync(movie);
        await sut.SaveChangesAsync();

        await ctx.Entry(movie).ReloadAsync();
        Assert.That(movie.Title, Is.EqualTo("Renamed"));
    }

    [Test]
    public async Task DeleteAsync_RemovesMovie_WhenExists()
    {
        await using var ctx = CreateContext();
        var movie = NewMovie("Gone", 2017, new DateTime(2017, 1, 1));
        ctx.Movies.Add(movie);
        await ctx.SaveChangesAsync();
        var id = movie.Id;
        var sut = new MovieRepository(ctx);

        await sut.DeleteAsync(id);
        await sut.SaveChangesAsync();

        Assert.That(await ctx.Movies.FindAsync(id), Is.Null);
    }

    [Test]
    public async Task DeleteAsync_Completes_WhenIdMissing()
    {
        await using var ctx = CreateContext();
        var sut = new MovieRepository(ctx);

        await sut.DeleteAsync(9999);
        await sut.SaveChangesAsync();

        Assert.That(await ctx.Movies.CountAsync(), Is.EqualTo(0));
    }
}
