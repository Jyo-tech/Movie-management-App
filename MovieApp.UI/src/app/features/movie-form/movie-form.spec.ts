import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { NgForm } from '@angular/forms';

import { MovieForm } from './movie-form';
import { MovieService } from '../../core/movie-service';
import { Notification } from '../../core/notificationService';
import { MovieDto } from '../../models/movie.model';

describe('MovieForm', () => {
  let component: MovieForm;
  let fixture: ComponentFixture<MovieForm>;
  let movieService: any;
  let notificationService: any;
  let router: any;
  let activatedRoute: any;

  beforeEach(async () => {
    movieService = {
      getMovieById: jest.fn(),
      createMovie: jest.fn(),
      updateMovie: jest.fn()
    };
    notificationService = {
      showError: jest.fn(),
      showSuccess: jest.fn()
    };
    router = { navigate: jest.fn() };
    activatedRoute = { snapshot: { paramMap: { get: jest.fn().mockReturnValue(null) } } };

    await TestBed.configureTestingModule({
  imports: [CommonModule, MovieForm], // ✅ MovieForm is standalone
  providers: [
    { provide: MovieService, useValue: movieService },
    { provide: Notification, useValue: notificationService },
    { provide: Router, useValue: router },
    { provide: ActivatedRoute, useValue: activatedRoute },
  ]
}).compileComponents();

    fixture = TestBed.createComponent(MovieForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize new movie with today releaseDate and default genre when no id', () => {
    const today = new Date().toISOString().split('T')[0];

    expect(component.isEditMode).toBe(false);
    expect(component.movie.releaseDate).toBe(today);
    expect(component.movie.genres).toEqual([component.availableGenres[0]]);
    expect(component.movie.runningTimeSecs).toBe(3600);
  });

  it('should load movie details when route has id param', () => {
    const mockMovie: MovieDto = {
      id: 5,
      title: 'Test',
      year: 2020,
      directors: 'Director',
      releaseDate: '2020-01-01',
      rating: 5,
      genres: ['Action'],
      actors: ['A', 'B'],
      imageUrl: '',
      plot: '',
      rank: 1,
      runningTimeSecs: 100
    };

    activatedRoute.snapshot.paramMap.get.mockReturnValue('5');
    movieService.getMovieById.mockReturnValue(of(mockMovie));

    component.ngOnInit();

    expect(component.isEditMode).toBe(true);
    expect(movieService.getMovieById).toHaveBeenCalledWith(5);
    expect(component.movie).toEqual(mockMovie);
    expect(component.actorsInput).toBe('A, B');
    expect(component.loading).toBe(false);
  });

  it('should not call API when form invalid', () => {
    const markAll = jest.fn();
    const invalidForm = {
      invalid: true,
      form: { markAllAsTouched: markAll },
    } as unknown as NgForm;

    component.onSubmit(invalidForm);

    expect(markAll).toHaveBeenCalled();
    expect(movieService.createMovie).not.toHaveBeenCalled();
    expect(component.loading).toBe(false);
  });

  it('should create movie and navigate on successful submit', () => {
    component.isEditMode = false;
    component.actorsInput = 'Actor One, Actor Two';
    component.movie = { ...component.movie, id: 0, title: 'New' } as any;
    const created = { id: 99 } as MovieDto;
    movieService.createMovie.mockReturnValue(of(created));

      const validMovieForm = () => ({
      invalid: false,
      form: { markAllAsTouched: jest.fn() },
    } as unknown as NgForm);

    component.onSubmit(validMovieForm());

    expect(movieService.createMovie).toHaveBeenCalled();
    expect(notificationService.showSuccess).toHaveBeenCalledWith('Movie created successfully!');
    expect(router.navigate).toHaveBeenCalledWith(['/movies', 99]);
  });
});


