import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';

import { MovieDetails } from './movie-details';
import { MovieService } from '../../core/movie-service';
import { ActivatedRoute, Router } from '@angular/router';
import { Notification } from '../../core/notificationService';

describe('MovieDetails', () => {
  let component: MovieDetails;
  let fixture: ComponentFixture<MovieDetails>;
  let movieService: any;
  let activatedRoute: any;
  let router: any;
  let notification: any;

  beforeEach(async () => {
    const movieServiceMock = {
      getMovieById: jest.fn(),
      deleteMovie: jest.fn()
    };
    const activatedRouteMock = {
      snapshot: { paramMap: { get: jest.fn() } }
    };
    const routerMock = { navigate: jest.fn() };
    const notificationMock = {
      showSuccess: jest.fn(),
      showError: jest.fn()
    };

    await TestBed.configureTestingModule({
      imports: [MovieDetails],
      providers: [
        { provide: MovieService, useValue: movieServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        { provide: Router, useValue: routerMock },
        { provide: Notification, useValue: notificationMock }
      ]
    })
    .compileComponents();

    movieService = TestBed.inject(MovieService);
    activatedRoute = TestBed.inject(ActivatedRoute);
    router = TestBed.inject(Router);
    notification = TestBed.inject(Notification);

    fixture = TestBed.createComponent(MovieDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should set error when id is invalid', () => {
    activatedRoute.snapshot.paramMap.get.mockReturnValue(null);

    component.ngOnInit();

    expect(component.error).toBe('Invalid Movie ID');
    expect(component.loading).toBe(false);
    expect(movieService.getMovieById).not.toHaveBeenCalled();
  });

  it('should load movie details when id is valid', () => {
    const mockMovie = { id: 1, title: 'Test Movie' };
    activatedRoute.snapshot.paramMap.get.mockReturnValue('1');
    movieService.getMovieById.mockReturnValue(of(mockMovie));

    component.ngOnInit();

    expect(movieService.getMovieById).toHaveBeenCalledWith(1);
    expect(component.movie).toEqual(mockMovie);
    expect(component.loading).toBe(false);
    expect(component.error).toBeNull();
  });

  it('should not delete movie when user cancels', () => {
    component.movie = { id: 1, title: 'Test Movie' } as any;
    jest.spyOn(window, 'confirm').mockReturnValue(false);

    component.onDelete();

    expect(movieService.deleteMovie).not.toHaveBeenCalled();
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('should delete movie and navigate when confirmed', () => {
    component.movie = { id: 1, title: 'Test Movie' } as any;
    movieService.deleteMovie.mockReturnValue(of(undefined));
    jest.spyOn(window, 'confirm').mockReturnValue(true);

    component.onDelete();

    expect(movieService.deleteMovie).toHaveBeenCalledWith(1);
    expect(notification.showSuccess).toHaveBeenCalledWith('Movie deleted successfully.');
    expect(router.navigate).toHaveBeenCalledWith(['/']);
  });
});
