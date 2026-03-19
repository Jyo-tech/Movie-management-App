import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { Search } from './features/Search/search';

export const routes: Routes = [
     { path: '', component: Home },
     { path: 'search', component: Search }

];
