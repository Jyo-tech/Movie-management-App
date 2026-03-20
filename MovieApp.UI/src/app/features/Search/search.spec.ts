import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, Router } from '@angular/router';
import { NgForm } from '@angular/forms';

import { Search } from './search';
import { of } from 'rxjs';

function validSearchForm(): NgForm {
  return {
    invalid: false,
    form: { markAllAsTouched: jest.fn() },
  } as unknown as NgForm;
}

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
    component.onSearch(validSearchForm());

    expect(router.navigate).toHaveBeenCalledWith(['/search/results'], {
      queryParams: { page: 1, title: 'Inception' },
    });
  });

  it('should navigate with title, year, and genre', () => {
    component.searchTitle = 'Inception';
    component.searchYear = 2010;
    component.searchGenre = 'Sci-Fi';
    component.onSearch(validSearchForm());

    expect(router.navigate).toHaveBeenCalledWith(['/search/results'], {
      queryParams: { page: 1, title: 'Inception', year: 2010, genre: 'Sci-Fi' },
    });
  });

  it('should not navigate when form invalid', () => {
    const markAll = jest.fn();
    const invalidForm = {
      invalid: true,
      form: { markAllAsTouched: markAll },
    } as unknown as NgForm;
    component.searchYear = 1700;
    component.onSearch(invalidForm);

    expect(markAll).toHaveBeenCalled();
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('should clear form and navigate to search without query params', () => {
    const resetForm = jest.fn();
    component.searchTitle = 'Inception';
    component.searchYear = 2010;
    component.searchGenre = 'Sci-Fi';

    component.clearSearch({ resetForm } as unknown as NgForm);

    expect(component.searchTitle).toBe('');
    expect(component.searchYear).toBeNull();
    expect(component.searchGenre).toBe('');
    expect(resetForm).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/search'], { queryParams: {} });
  });
});
