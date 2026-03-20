namespace MovieApp.Application.DTOs;

public class PagedMoviesDto
{
    public List<MovieDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
