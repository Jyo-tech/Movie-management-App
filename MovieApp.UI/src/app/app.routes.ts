import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { MovieDetails } from './features/movie-details/movie-details';
import { MovieForm } from './features/movie-form/movie-form';
import { Search } from './features/Search/search';

export const routes: Routes = [
      { path: '', component: Home },
  { path: 'search', component: Search },
  { path: 'movies/add', component: MovieForm },
  { path: 'movies/edit/:id', component: MovieForm },
  { path: 'movies/:id', component: MovieDetails },
  { path: 'movies/add', redirectTo: '', pathMatch: 'full' },
  { path: '**', redirectTo: '' }

];
