import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, Router } from '@angular/router';
import { of } from 'rxjs';

import { Search } from './search';

describe('Search', () => {
  let component: Search;
  let fixture: ComponentFixture<Search>;
  let router: Router;

  beforeEach(async () => {
    const routerMock = { navigate: jest.fn() };

    await TestBed.configureTestingModule({
      imports: [Search],
      providers: [
        { provide: Router, useValue: routerMock },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: { queryParamMap: convertToParamMap({}) },
            queryParamMap: of(convertToParamMap({})),
          },
        },
      ],
    }).compileComponents();

    router = TestBed.inject(Router);
    fixture = TestBed.createComponent(Search);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate to search results with title', () => {
    component.searchTitle = 'Inception';
    component.onSearch();

    expect(router.navigate).toHaveBeenCalledWith(['/search/results'], {
      queryParams: { page: 1, title: 'Inception' },
    });
  });

  it('should navigate with title, year, and genre', () => {
    component.searchTitle = 'Inception';
    component.searchYear = 2010;
    component.searchGenre = 'Sci-Fi';
    component.onSearch();

    expect(router.navigate).toHaveBeenCalledWith(['/search/results'], {
      queryParams: { page: 1, title: 'Inception', year: 2010, genre: 'Sci-Fi' },
    });
  });

  it('should clear form and navigate to search without query params', () => {
    component.searchTitle = 'Inception';
    component.searchYear = 2010;
    component.searchGenre = 'Sci-Fi';

    component.clearSearch();

    expect(component.searchTitle).toBe('');
    expect(component.searchYear).toBeNull();
    expect(component.searchGenre).toBe('');
    expect(router.navigate).toHaveBeenCalledWith(['/search'], { queryParams: {} });
  });
});
