import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MovieDto } from '../models/movie.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class MovieService {
  constructor(protected readonly http: HttpClient) {

   }

   getLatestMovies(count: number) : Observable<MovieDto[]> {
    return this.http.get<MovieDto[]>(environment.apiUrl + '/latest');
   }

   searchMovies(title?: string, genre?: string, year?: number): Observable<MovieDto[]> {
    let params = new HttpParams();
    if (title) params = params.set('title', title);
    if (genre) params = params.set('genre', genre);
    if (year) params = params.set('year', year.toString());

    return this.http.get<MovieDto[]>(`${environment.apiUrl}/search`, { params });
  }

  getMovieById(id: number): Observable<MovieDto> {
    return this.http.get<MovieDto>(`${environment.apiUrl}/${id}`);
  }

  deleteMovie(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/${id}`);
  }
  updateMovie( id: number, movie: MovieDto): Observable<MovieDto> {
    return this.http.put<MovieDto>(`${environment.apiUrl}/${id}`, movie);
  }

  createMovie(movie: MovieDto): Observable<MovieDto> {
    return this.http.post<MovieDto>(environment.apiUrl, movie);
  }
}
