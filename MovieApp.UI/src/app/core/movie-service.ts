import { HttpClient } from '@angular/common/http';
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
  
}
