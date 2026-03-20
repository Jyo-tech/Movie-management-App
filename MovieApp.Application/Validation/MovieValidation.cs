using FluentValidation;
using MovieApp.Application.DTOs;

namespace MovieApp.Application.Validation
{
    public class MovieValidation : AbstractValidator<MovieDto>
    {
        public MovieValidation()
        {
             RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Year)
            .InclusiveBetween(1900, 2100)
            .WithMessage("Year must be between 1900 and 2100.");

        RuleFor(x => x.Directors)
            .NotEmpty().WithMessage("Directors are required.");

        RuleFor(x => x.ReleaseDate)
            .NotEmpty().WithMessage("Release date is required.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(0.0, 10.0)
            .WithMessage("Rating must be between 0 and 10.");

        RuleFor(x => x.Genres)
            .NotNull().WithMessage("Genres are required.")
            .Must(g => g.Count > 0).WithMessage("At least one genre is required.");

        RuleFor(x => x.Actors)
            .NotNull().WithMessage("Actors are required.")
            .Must(a => a.Count > 0).WithMessage("At least one actor is required.");

        RuleFor(x => x.ImageUrl)
            .Must(uri => string.IsNullOrEmpty(uri) || Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            .WithMessage("Image URL must be a valid URL.");

        RuleFor(x => x.Plot)
            .MaximumLength(1000).WithMessage("Plot cannot exceed 1000 characters.");

        RuleFor(x => x.Rank)
            .GreaterThan(0).WithMessage("Rank must be greater than 0.");

        RuleFor(x => x.RunningTimeSecs)
            .GreaterThan(0).WithMessage("Running time must be greater than 0.");


        }
    }
}