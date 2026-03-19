import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms'; 
import { MovieService } from '../../core/movie-service';
import { MovieDto } from '../../models/movie.model';
import { Notification } from '../../core/notification';
@Component({
  selector: 'app-movie-form',
  imports: [CommonModule , FormsModule , RouterModule],
  templateUrl: './movie-form.html',
  styleUrl: './movie-form.css',
})

export class MovieForm implements OnInit {
  isEditMode: boolean = false;
  loading = false;
  actorsInput: string = '';
  
  availableGenres: string[] = ['Action', 'Comedy', 'Drama', 'Horror', 'Sci-Fi', 'Thriller', 'Romance', 'Documentary', 'Crime', 'Mystery', 'History', 'War'];

  movie: MovieDto = {
    id: 0,
    title: '',
    year: new Date().getFullYear(),
    directors: '',
    releaseDate: '',
    rating: 0,
    genres: [],
    actors: [],
    imageUrl: '',
    plot: '',
    rank: 1,
    runningTimeSecs: 0
  };

  constructor(
    private movieService: MovieService,
    private router: Router,
      private notificationService: Notification,
      private route: ActivatedRoute
  ) {

  }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');   
     if (idParam) {
      this.isEditMode = true;
      this.loadMovie(Number(idParam));
  }
  else{
       this.movie.releaseDate = new Date().toISOString().split('T')[0];
       this.movie.genres = [this.availableGenres[0]];
  }

}

  loadMovie(id: number): void {
    this.loading = true;
    this.movieService.getMovieById(id).subscribe({
      next: (movie) => {
        this.movie = movie;
               if (this.movie.releaseDate) {
           this.movie.releaseDate = new Date(this.movie.releaseDate).toISOString().split('T')[0];
        }

        this.actorsInput = this.movie.actors.join(', ');
        
        this.loading = false;
      },
      error: () => {
        this.notificationService.showError('Failed to load movie for editing.');
        this.router.navigate(['/']);
      }
    });
  }

    onSubmit(): void {
    if (this.loading) return;
    this.loading = true;

    this.movie.actors = this.actorsInput
      .split(',')
      .map(a => a.trim())
      .filter(a => a.length > 0);

    if (this.movie.actors.length === 0) {
      this.notificationService.showError('Actors must not be empty.');
      this.loading = false;
      return;
    }

    if (this.isEditMode) {
      this.movieService.updateMovie(this.movie.id, this.movie).subscribe({
        next: () => {
          this.notificationService.showSuccess('Movie updated successfully!');
          this.router.navigate(['/movies', this.movie.id]);
        },
        error: (err) => this.handleError(err)
      });
    } else {
      this.movieService.createMovie(this.movie).subscribe({
        next: (created) => {
          this.notificationService.showSuccess('Movie created successfully!');
          this.router.navigate(['/movies', created.id]);
        },
        error: (err) => this.handleError(err)
      });
    }
  }

  private handleError(error: any): void {
    this.loading = false;
    console.error('API Error', error);
    
    if (error.status === 400 && error.error && error.error.errors) {
       const validationErrors = Object.values(error.error.errors).flat().join(' ');
       this.notificationService.showError(`Validation failed: ${validationErrors}`);
    } else {
       this.notificationService.showError('An unexpected error occurred while saving the movie.');
    }
  }

}
