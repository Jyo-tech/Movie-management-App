import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router, RouterModule } from '@angular/router';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
  selector: 'app-search',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './search.html',
  styleUrl: './search.css',
})
export class Search implements OnInit, OnDestroy {
  searchTitle = '';
  searchYear: number | null = null;
  searchGenre = '';

  availableGenres: string[] = [
    'Action',
    'Comedy',
    'Drama',
    'Horror',
    'Sci-Fi',
    'Thriller',
    'Romance',
  ];

  private querySub?: Subscription;

  constructor(
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.applyQueryParams(this.route.snapshot.queryParamMap);
    this.querySub = this.route.queryParamMap.subscribe((q) =>
      this.applyQueryParams(q)
    );
  }

  ngOnDestroy(): void {
    this.querySub?.unsubscribe();
  }

  private applyQueryParams(q: ParamMap): void {
    const title = q.get('title');
    this.searchTitle = title !== null ? title : '';

    const genre = q.get('genre');
    this.searchGenre = genre !== null ? genre : '';

    const y = q.get('year');
    if (y != null && y !== '') {
      const n = Number(y);
      this.searchYear = Number.isNaN(n) ? null : n;
    } else {
      this.searchYear = null;
    }
  }

  onSearch(form: NgForm): void {
    if (form.invalid) {
      form.form.markAllAsTouched();
      return;
    }

    const title = this.searchTitle.trim();
    const queryParams: Record<string, string | number> = { page: 1 };
    if (title) queryParams['title'] = title;
    if (this.searchYear != null && !Number.isNaN(this.searchYear)) {
      queryParams['year'] = this.searchYear;
    }
    if (this.searchGenre) queryParams['genre'] = this.searchGenre;

    void this.router.navigate(['/search/results'], { queryParams });
  }

  clearSearch(form?: NgForm): void {
    this.searchTitle = '';
    this.searchYear = null;
    this.searchGenre = '';
    form?.resetForm({
      title: '',
      year: null,
      genre: '',
    });
    void this.router.navigate(['/search'], { queryParams: {} });
  }
}

