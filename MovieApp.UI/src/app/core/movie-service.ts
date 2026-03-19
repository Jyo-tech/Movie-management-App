import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MovieDto } from '../models/movie.model';

@Injectable({
  providedIn: 'root',
})
export class MovieService {
  readonly baseUrl = 'https://localhost:7267/api/movies';
  constructor(protected readonly http: HttpClient) {

   }

   getLatestMovies(count: number) : Observable<MovieDto[]> {
    return this.http.get<MovieDto[]>(this.baseUrl + '/latest');
   }
  
}
