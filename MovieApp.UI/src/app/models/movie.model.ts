export interface MovieDto {
    id: number;
    title: string;
    year: number;
    directors: string;
    releaseDate: string;
    rating: number;
    genres: string[];
    actors: string[];
    imageUrl: string;
    plot: string;
    rank: number;
    runningTimeSecs: number;
}