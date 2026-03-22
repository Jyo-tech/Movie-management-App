import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, Router } from '@angular/router';
import { of } from 'rxjs';

import { SearchResults } from './search-results';
import { MovieService } from '../../core/movie-service';

const emptyPage = { items: [], totalCount: 0, page: 1, pageSize: 24 };

describe('SearchResults', () => {
  let component: SearchResults;
  let fixture: ComponentFixture<SearchResults>;
  let movieService: any;
  let router: any;

  beforeEach(async () => {
    const movieServiceMock = { searchMovies: jest.fn() };
    const routerMock = { navigate: jest.fn() };
    const activatedRouteMock = {
      queryParamMap: of(convertToParamMap({ page: '1', title: 'Test' })),
    };

    await TestBed.configureTestingModule({
      imports: [SearchResults],
      providers: [
        { provide: MovieService, useValue: movieServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
      ],
    }).compileComponents();

    movieService = TestBed.inject(MovieService);
    router = TestBed.inject(Router);
    movieService.searchMovies.mockReturnValue(of(emptyPage));

    fixture = TestBed.createComponent(SearchResults);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });


  it('should update page via router', () => {
    component.totalCount = 100;
    component.page = 2;

    component.goToPage(3);

    expect(router.navigate).toHaveBeenCalledWith(
      [],
      expect.objectContaining({
        queryParams: { page: 3 },
        queryParamsHandling: 'merge',
      })
    );
  });

    it('should call search on NgOnit', () => {
    component.totalCount = 100;
    component.page = 2;

    component.ngOnInit();

    expect(movieService.searchMovies).toHaveBeenCalledWith(
      'Test',
      undefined,
      undefined,
      1,
      24
    );
  });

 
});
