using MovieApp.Domain.Entities;
using NUnit.Framework;

namespace MovieApp.Domain.Tests.Entities;

[TestFixture]
public class MovieTests
{
    [Test]
    public void NewMovie_HasEmptyCollections_AndDefaultStrings()
    {
        var movie = new Movie();

        Assert.That(movie.Title, Is.EqualTo(string.Empty));
        Assert.That(movie.Directors, Is.EqualTo(string.Empty));
        Assert.That(movie.Genres, Is.Not.Null);
        Assert.That(movie.Genres, Is.Empty);
        Assert.That(movie.Actors, Is.Not.Null);
        Assert.That(movie.Actors, Is.Empty);
    }

    [Test]
    public void Movie_RoundTrips_AssignedValues()
    {
        var release = new DateTime(2021, 3, 15);
        var movie = new Movie
        {
            Id = 10,
            Title = "Sample",
            Year = 2021,
            Directors = "Director A",
            ReleaseDate = release,
            Rating = 8.2,
            Genres = new string[] { "Drama", "Thriller" },
            Actors = new List<string> { "Actor 1" },
            ImageUrl = "https://example.com/x.jpg",
            Plot = "Plot text",
            Rank = 2,
            RunningTimeSecs = 5400
        };

        Assert.That(movie.Id, Is.EqualTo(10));
        Assert.That(movie.Title, Is.EqualTo("Sample"));
        Assert.That(movie.Genres, Has.Length.EqualTo(2));
        Assert.That(movie.Genres[0], Is.EqualTo("Drama"));
        Assert.That(movie.ReleaseDate, Is.EqualTo(release));
        Assert.That(movie.RunningTimeSecs, Is.EqualTo(5400));
    }
}
