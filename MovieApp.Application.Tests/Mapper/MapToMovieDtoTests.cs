using MovieApp.Application.DTOs;
using MovieApp.Application.Mapper;
using MovieApp.Domain.Entities;
using NUnit.Framework;

namespace MovieApp.Application.Tests.Mapper;

[TestFixture]
public class MapToMovieDtoTests
{
    private readonly MapToMovieDto _mapper = new();

    [Test]
    public void MapToDto_Copies_AllProperties()
    {
        var release = new DateTime(2019, 7, 4);
        var entity = new Movie
        {
            Id = 5,
            Title = "Inception",
            Year = 2010,
            Directors = "Christopher Nolan",
            ReleaseDate = release,
            Rating = 8.8,
            Genres = new string [] {"Sci-Fi", "Thriller" },
            Actors = new List<string> { "Leonardo DiCaprio" },
            ImageUrl = "https://example.com/inception.jpg",
            Plot = "Dreams within dreams.",
            Rank = 14,
            RunningTimeSecs = 8880
        };

        MovieDto dto = _mapper.MapToDto(entity);

        Assert.That(dto.Id, Is.EqualTo(5));
        Assert.That(dto.Title, Is.EqualTo("Inception"));
        Assert.That(dto.Genres, Is.EquivalentTo(new[] { "Sci-Fi", "Thriller" }));
        Assert.That(dto.Actors, Is.EquivalentTo(new[] { "Leonardo DiCaprio" }));
        Assert.That(dto.ReleaseDate, Is.EqualTo(release));
        Assert.That(dto.Rating, Is.EqualTo(8.8));
        Assert.That(dto.RunningTimeSecs, Is.EqualTo(8880));
    }
}
