GalaxAI Game API (ASP.NET Core) — scaffold placeholders

Controllers to implement:
- GET /player/{id}/state
- POST /action
- GET /world/summary
- GET /lore/{id}
- POST /auth/keys (create)
- GET /auth/keys (list)
- POST /auth/keys/rotate
- DELETE /auth/keys/{id}

Storage: Azure Table Storage (ETag optimistic concurrency). Implement interfaces in Game.Storage to swap storage backends.

To generate the project locally:
1) dotnet new webapi -n GalaxAI.Game.Api
2) dotnet add package Azure.Data.Tables
3) Implement controllers and services as per README top-level.
4) Add Dockerfile and run with docker-compose (see root docker-compose.yml)
