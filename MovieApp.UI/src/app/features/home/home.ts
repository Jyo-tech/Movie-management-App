import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { MovieService } from '../../core/movie-service';
import { MovieDto } from '../../models/movie.model';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [CommonModule ,RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit {
  loading: boolean = true;
  latestMovies: MovieDto[] = [];
  error: string | null = null;

  constructor(
    protected readonly movieService: MovieService
  ) {}

  ngOnInit(): void {
    this.getLatestMovies();
  }

  getLatestMovies() {
    this.movieService.getLatestMovies(4).subscribe({
      next: (movies) => {
        console.log(movies);
        this.latestMovies = movies;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        console.log('Failed to load latest movies. Please try again later.');
        this.error = 'Failed to load latest movies. Please try again later.';
        this.loading = false;
      }
    });
  }
}
