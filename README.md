# Music.Catalogue

CatalogueService — the Spotify-facing microservice of the [Music Microservices](https://github.com/lucatam05/Music_Compose) project. It exposes song search endpoints backed by the Spotify Web API, with a Postgres cache in front of it.

> Looking to run the full stack? Start from [Music_Compose](https://github.com/lucatam05/Music_Compose).

## Responsibilities

- Search songs by title, artist, or album
- Talk to the Spotify Web API (OAuth2 Client Credentials flow) and cache results in Postgres, keyed by Spotify ID (cache-aside pattern)
- Serve as the authoritative song-metadata source for the other services

## Project layout

```
Music.Catalogue.WebApi        → HTTP API, DI composition root, resilience/health/logging wiring
Music.Catalogue.Business       → use cases
Music.Catalogue.Repository     → EF Core + Postgres cache
Music.Catalogue.Spotify        → internal client talking to the Spotify API + OAuth token caching
Music.Catalogue.ClientHttp     → typed HTTP client published as a NuGet package, consumed by LibraryService/UserService
Music.Catalogue.Shared         → DTOs shared with consumers of the ClientHttp package
```

`Spotify` and `ClientHttp` are deliberately separate: the former talks to Spotify, the latter is what *other services* use to talk to *this* service. Conflating them made an earlier version of this service harder to reason about.

## Resilience

Calls to Spotify go through a Polly-based pipeline (`SpotifyResilienceExtensions.AddSpotifyHttpClients`):

- Retry: 3 attempts, exponential backoff + jitter
- Circuit breaker: opens above 50% failure ratio in a 30s window (min. 4 requests), breaks for 15s
- Per-attempt timeout: 5s / total timeout: 20s

The Spotify OAuth token is cached in memory (`SpotifyTokenProvider`), with a 60s safety margin before real expiry and double-checked locking to avoid a thundering herd of token requests.

## Observability

- **Structured logging** via Serilog, enriched with `ServiceName` and `CorrelationId`
- **Correlation ID**: read from the incoming `X-Correlation-Id` header (or generated if absent), pushed into the Serilog log context for the whole request
- **Health check** — `GET /health`:
  - `database`: Postgres connectivity via the DbContext
  - `spotify`: verifies a valid OAuth token can be obtained (cheap in practice, since the token is cached — this only makes a real Spotify call when the cached token has actually expired)

## API

Base route: `/Catalogue`

| Method | Route | Description |
|---|---|---|
| GET | `/Catalogue/GetCanzoniPerNome?titolo=` | Search songs by title |
| GET | `/Catalogue/GetCanzoniPerArtista?artista=` | Search songs by artist |
| GET | `/Catalogue/GetCanzoniPerAlbum?album=` | Search songs by album |

Full request/response schemas are on Swagger at `/swagger` once the service is running.

## Configuration

| Setting | Purpose |
|---|---|
| `ConnectionStrings:DefaultConnection` | Postgres connection string |
| `Spotify:ClientId` / `Spotify:ClientSecret` | Spotify app credentials |
| `Spotify:TokenUrl` / `Spotify:SearchUrl` / `Spotify:TrackUrl` | Spotify API endpoints |
| `Serilog:MinimumLevel:Default` | Log verbosity, overridable via `LOG_LEVEL` env var in Compose |

In the full stack, all of this is wired via `Music_Compose`'s `docker-compose.yml` and `.env`. For standalone local development, use `dotnet user-secrets` for the Spotify credentials rather than committing them anywhere.

## Local development

```bash
dotnet restore
dotnet ef database update --project Music.Catalogue.Repository --startup-project Music.Catalogue.WebApi
dotnet run --project Music.Catalogue.WebApi
```

Requires a running Postgres instance and Spotify Developer credentials (see [Music_Compose](https://github.com/lucatam05/Music_Compose) for the easiest way to get a full local environment up, including Postgres).
