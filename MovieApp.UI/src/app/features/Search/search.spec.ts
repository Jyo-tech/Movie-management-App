import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';

import { Search } from './search';
import { MovieService } from '../../core/movie-service';

describe('Search', () => {
  let component: Search;
  let fixture: ComponentFixture<Search>;
  let movieService: any;

  beforeEach(async () => {
    const movieServiceMock = {
      searchMovies: jest.fn()
    };

    await TestBed.configureTestingModule({
      imports: [Search],
      providers: [
        { provide: MovieService, useValue: movieServiceMock }
      ]
    })
    .compileComponents();
    movieService = TestBed.inject(MovieService);

    fixture = TestBed.createComponent(Search);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call searchMovies on onSearch with title', () => {
    const searchQuery = 'Inception';
    component.searchTitle = searchQuery;
    movieService.searchMovies.mockReturnValue(of([]));
    component.onSearch();

    expect(component.loading).toBe(false);
    expect(component.error).toBeNull();
    expect(component.hasSearched).toBe(true);
    expect(movieService.searchMovies).toHaveBeenCalledWith(searchQuery, undefined, undefined);
  });

  it('should call searchMovies on onSearch with title, year, and genre', () => {
    component.searchTitle = 'Inception';
    component.searchYear = 2010;
    component.searchGenre = 'Sci-Fi';
    movieService.searchMovies.mockReturnValue(of([]));
    component.onSearch();

    expect(component.loading).toBe(false);
    expect(component.error).toBeNull();
    expect(component.hasSearched).toBe(true);
    expect(movieService.searchMovies).toHaveBeenCalledWith('Inception', 'Sci-Fi', 2010);
  });

  it('should handle search success and set movies', () => {
    const mockMovies = [{ title: 'Inception' }];
    movieService.searchMovies.mockReturnValue(of(mockMovies));
    component.onSearch();

    expect(component.movies).toEqual(mockMovies);
    expect(component.loading).toBe(false);
    expect(component.error).toBeNull();
    expect(component.hasSearched).toBe(true);
  });

  it('should handle search error', () => {
    movieService.searchMovies.mockReturnValue(throwError(() => new Error('API Error')));
    component.onSearch();

    expect(component.error).toBe('Failed to search movies. Please try again later.');
    expect(component.loading).toBe(false);
    expect(component.hasSearched).toBe(true);
  });

  it('should clear search', () => {
    component.searchTitle = 'Inception';
    component.searchYear = 2010;
    component.searchGenre = 'Sci-Fi';
    component.movies = [{ title: 'Inception' }];
    component.error = 'Error';
    component.hasSearched = true;

    component.clearSearch();

    expect(component.searchTitle).toBe('');
    expect(component.searchYear).toBeNull();
    expect(component.searchGenre).toBe('');
    expect(component.movies).toEqual([]);
    expect(component.error).toBeNull();
    expect(component.hasSearched).toBe(false);
  });
});
