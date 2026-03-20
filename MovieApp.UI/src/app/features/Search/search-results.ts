import { Component, NgZone, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { MovieDto } from '../../models/movie.model';
import { MovieService } from '../../core/movie-service';

@Component({
  selector: 'app-search-results',
  imports: [CommonModule, RouterLink],
  templateUrl: './search-results.html',
  styleUrl: './search-results.css',
})
export class SearchResults implements OnInit, OnDestroy {
  movies: MovieDto[] = [];
  totalCount = 0;
  page = 1;
  readonly pageSize = 24;
  loading = false;
  error: string | null = null;
  hasLoaded = false;

  /** Mirrors URL — for summary UI */
  filterTitle = '';
  filterGenre = '';
  filterYear: number | null = null;

  private querySub?: Subscription;

  constructor(
    private movieService: MovieService,
    private route: ActivatedRoute,
    private router: Router,
    private zone: NgZone
  ) {}

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalCount / this.pageSize));
  }

  ngOnInit(): void {
    this.querySub = this.route.queryParamMap.subscribe((params) => {
      const pageRaw = params.get('page');
      const page = Math.max(1, Number(pageRaw || 1) || 1);

      const title = params.get('title')?.trim() || '';
      const genre = params.get('genre')?.trim() || '';
      let year: number | null = null;
      const y = params.get('year');
      if (y != null && y !== '') {
        const n = Number(y);
        if (!Number.isNaN(n)) year = n;
      }

      this.filterTitle = title;
      this.filterGenre = genre;
      this.filterYear = year;

      const titleParam = title || undefined;
      const genreParam = genre || undefined;
      const yearParam = year ?? undefined;

      this.runSearch(titleParam, genreParam, yearParam, page);
    });
  }

  ngOnDestroy(): void {
    this.querySub?.unsubscribe();
  }

  goToPage(p: number): void {
    const next = Math.max(1, Math.min(p, this.totalPages));
    if (next === this.page) return;
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { page: next },
      queryParamsHandling: 'merge',
    });
  }

  private runSearch(
    title: string | undefined,
    genre: string | undefined,
    year: number | undefined,
    page: number
  ): void {
    this.loading = true;
    this.error = null;
    this.hasLoaded = false;

    this.movieService.searchMovies(title, genre, year, page, this.pageSize).subscribe({
      next: (result) => {
        this.zone.run(() => {
          this.movies = result.items;
          this.totalCount = result.totalCount;
          this.page = result.page;
          this.loading = false;
          this.hasLoaded = true;
        });
      },
      error: () => {
        this.zone.run(() => {
          this.error = 'Failed to search movies. Please try again later.';
          this.loading = false;
          this.hasLoaded = true;
        });
      },
    });
  }
}
