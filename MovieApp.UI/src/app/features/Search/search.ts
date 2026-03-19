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
  loading: boolean = false;
  error: string | null = null;
  hasSearched: boolean = false;

  availableGenres: string[] = ['Action', 'Comedy', 'Drama', 'Horror', 'Sci-Fi', 'Thriller', 'Romance'];

  constructor(private movieService: MovieService , private zone: NgZone) {}

  onSearch() {
    this.loading = true;
    this.error = null;
    this.hasSearched = true;

    const titleParam = this.searchTitle.trim() || undefined;
    const genreParam = this.searchGenre || undefined;
    const yearParam = this.searchYear || undefined;

    this.movieService.searchMovies(titleParam, genreParam, yearParam).subscribe({
      next: (movies) => {
        this.zone.run(() => {
        this.movies = movies;
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
    this.error = null;
    this.hasSearched = false;
  }

}
