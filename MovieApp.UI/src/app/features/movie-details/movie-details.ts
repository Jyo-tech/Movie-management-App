import { Component, OnInit } from '@angular/core';
import { MovieService } from '../../core/movie-service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MovieDto } from '../../models/movie.model';
import { CommonModule } from '@angular/common';
import { Notification } from '../../core/notification';

@Component({
  selector: 'app-movie-details',
  imports: [CommonModule ,RouterLink],
  templateUrl: './movie-details.html',
  styleUrl: './movie-details.css',
})
export class MovieDetails implements OnInit {
  movie: MovieDto | null = null;
  loading: boolean = true
  error: string | null = null;

  constructor(
    private movieService: MovieService,
    private route: ActivatedRoute,
    private router: Router,
    private notification: Notification
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.fetchMovieDetails(id);
    } else {
      this.error = 'Invalid Movie ID';
      this.loading = false;
    }

}

  fetchMovieDetails(id: number) {
    this.movieService.getMovieById(id).subscribe({
      next: (movie) => {
        this.movie = movie;
        this.loading = false;
      }
      ,
      error: (err) => {
        console.error(err); 
        this.error = 'Failed to load movie details. Please try again later.';
        this.loading = false;
      }
    });   
}

onDelete(): void {
    if (!this.movie) return;

    if (confirm(`Are you sure you want to delete "${this.movie.title}"?`)) {
      this.movieService.deleteMovie(this.movie.id).subscribe({
        next: () => {
          this.notification.showSuccess('Movie deleted successfully.');
          this.router.navigate(['/']);
        },
        error: (err) => {
          console.error('Delete failed', err);
          this.notification.showError('Failed to delete the movie. Please try again.');
        }
      });
    }
  }
}
