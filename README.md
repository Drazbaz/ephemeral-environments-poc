# Ephemeral Environments PoC

A proof-of-concept ASP.NET Core API demonstrating:

- Dockerised API
- PostgreSQL
- EF Core migrations
- Repository pattern
- Unit of Work
- Test containers

## Running

```sh
docker compose up --build
```

Open http://localhost:3000 for the web app. The web container serves the built React app and proxies `/api` requests to the API container. The API and PostgreSQL start in the same Compose stack.

## API

GET /api/user <br>
POST /api/user

## Tech Stack

- .NET
- PostgreSQL
- Docker
- EF Core
- React
