# Movie Management Application

Full-stack app: **ASP.NET Core** REST API, **PostgreSQL**, **Angular** UI. Clean separation across **API → Application → Domain ← Infrastructure** layers.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download) (or the SDK version pinned by the solution)
- [Node.js](https://nodejs.org/) (LTS; matches Angular CLI in `MovieApp.UI`)
- [PostgreSQL](https://www.postgresql.org/download/) running locally

## Configure database

1. Create a database user and database (or adjust to match your environment).
2. Set the connection string in `MovieApp.API/appsettings.json` (or use [user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for local development):

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=MovieDb;Username=movie;Password=secret123"
}
```

3. Apply migrations from the repo root:

```bash
dotnet ef database update --project MovieApp.Infrastructure --startup-project MovieApp.API
```

## Run the API

```bash
cd MovieApp.API
dotnet run
```

Note the HTTPS/HTTP URLs printed in the console (e.g. `http://localhost:5121`). Ensure `MovieApp.UI/src/environments/environment.ts` has `apiUrl` pointing at `…/api/movies` on that host/port.

Swagger UI is available in Development (typically `/swagger`).

## Run the Angular UI

```bash
cd MovieApp.UI
npm install
ng serve
```

Open `http://localhost:4200`.

## Seed data

- **Source file:** `MovieApp.Infrastructure/Data/Seed/moviedata.json` (also linked into the API output as `Data/Seed/moviedata.json`).
- **Runtime seeding:** On startup, the API runs `DbSeeder` when `Seed:Enabled` is true (default **true** in `appsettings.Development.json`, **false** in production `appsettings.json`).
- **Idempotent:** If the `Movies` table already has rows, seeding is skipped unless you set **`Seed:ForceFullReseed` to `true` in Development**, which truncates the table and reloads from JSON (never honored outside Development).
- **Migrations:** Schema comes from EF migrations only; data is **not** embedded in `HasData`. After `database update`, start the API once (or use ForceFullReseed) to populate.

Optional migration `RemoveLegacyEmbeddedSeedRow` removes the old single-row migration seed if you upgraded from an earlier snapshot that inserted Gettysburg.

## Cors Issue 
check for appsetting.json to add allwed origins whichever port your localhots is running

## Tests

**Backend** (from repo root):

```bash
dotnet test
```

**Frontend** (`MovieApp.UI`):

```bash
ng test
```

## Architecture (short)

| Project | Role |
|--------|------|
| `MovieApp.Domain` | Entities, repository interfaces |
| `MovieApp.Application` | DTOs, `MovieService`, mapping |
| `MovieApp.Infrastructure` | EF Core `AppDbContext`, repositories, migrations, seed JSON |
| `MovieApp.API` | Controllers, DI wiring, HTTP |
| `MovieApp.UI` | Angular features, `MovieService`, shared notification toast |

## Assumptions and trade-offs

- **Running time** is stored as **seconds** (`RunningTimeSecs`), not a `HH:mm:ss` string; the API contract and UI use that shape consistently.
- **Search** uses **exact** (case-insensitive) title and genre token match, with optional year filter—good for precise lookups and indexing; partial “contains” search is intentionally not used.
- **Latest movies** on the home page are ordered by **release year** (newest first), which aligns with “latest” behaviour for typical datasets.
- **CORS** is configured for local Angular origins; tighten or use environment-based policies for deployment.
- **Database seeding** is done at API startup from JSON (not `HasData` migrations) so large datasets stay out of migration history.

## Key assignment alignment

- CRUD, validation on API (`MovieDto` data annotations) and Angular forms, search with dedicated results area, details with edit/delete (delete confirmation), toast notifications for feedback, and unit tests on API/application/infrastructure/domain and selected UI specs.
For better retrieval index have been added 

## Improvement 
- Add autherization/Authentication
- Add caching if continues search on for same movie 

