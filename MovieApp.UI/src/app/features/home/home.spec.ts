import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Home } from './home';
import { MovieService } from '../../core/movie-service';
import { throwError } from 'rxjs';

describe('Home', () => {
  let component: Home;
  let fixture: ComponentFixture<Home>;
  let movieService: any;

  beforeEach(async () => {
    const movieServiceMock = {
      getLatestMovies: jest.fn()
    };

    await TestBed.configureTestingModule({
      imports: [Home],
      providers: [{ provide: MovieService, useValue: movieServiceMock }]
    }).compileComponents();

    movieService = TestBed.inject(MovieService);
    fixture = TestBed.createComponent(Home);
    component = fixture.componentInstance;
  });

  it('should display error message when getLatestMovies fails', () => {
    movieService.getLatestMovies.mockReturnValue(throwError(() => new Error('API Error')));

    fixture.detectChanges();

    expect(component.error).toBe('Failed to load latest movies. Please try again later.');
    expect(component.loading).toBe(false);
  });
});