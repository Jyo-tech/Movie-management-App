import { Component, NgZone } from '@angular/core';
import { MovieDto } from '../../models/movie.model';
import { MovieService } from '../../core/movie-service';
import { CommonModule  } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-search',
  imports: [CommonModule ,FormsModule, RouterModule],
  templateUrl: './search.html',
  styleUrl: './search.css',
})
export class Search {
  searchTitle: string = '';
  searchYear: number | null = null;
  searchGenre: string = '';

  movies: MovieDto[] = [];
  totalCount = 0;
  page = 1;
  readonly pageSize = 24;
  loading: boolean = false;
  error: string | null = null;
  hasSearched: boolean = false;

  availableGenres: string[] = ['Action', 'Comedy', 'Drama', 'Horror', 'Sci-Fi', 'Thriller', 'Romance'];

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalCount / this.pageSize));
  }

  constructor(private movieService: MovieService , private zone: NgZone) {}

  onSearch() {
    this.page = 1;
    this.runSearch();
  }

  goToPage(p: number) {
    const next = Math.max(1, Math.min(p, this.totalPages));
    if (next === this.page) return;
    this.page = next;
    this.runSearch();
  }

  private runSearch() {
    this.loading = true;
    this.error = null;
    this.hasSearched = true;

    const titleParam = this.searchTitle.trim() || undefined;
    const genreParam = this.searchGenre || undefined;
    const yearParam = this.searchYear || undefined;

    this.movieService.searchMovies(titleParam, genreParam, yearParam, this.page, this.pageSize).subscribe({
      next: (result) => {
        this.zone.run(() => {
        this.movies = result.items;
        this.totalCount = result.totalCount;
        this.page = result.page;
        this.loading = false;
      });
      },
      error: (err) => {
        this.zone.run(() => {
          this.error = 'Failed to search movies. Please try again later.';
          this.loading = false;
        });
      }
    });

  }

  clearSearch() {
    this.searchTitle = '';
    this.searchYear = null;
    this.searchGenre = '';
    this.movies = [];
    this.totalCount = 0;
    this.page = 1;
    this.error = null;
    this.hasSearched = false;
  }

}
