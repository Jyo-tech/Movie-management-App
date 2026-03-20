import { MovieDto } from './movie.model';

export interface PagedMoviesDto {
  items: MovieDto[];
  totalCount: number;
  page: number;
  pageSize: number;
}
