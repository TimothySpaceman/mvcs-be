# MVCS Backend

This repo contains MVCS Backend application and some infrastructure.

## Requirements

- Docker, Docker Compose

## Dev Deployment

Make sure you have all env variables set up in `.env`:

- INTERNAL_NUGET_HOST - Host for package registry to pull Mvcs.Core image from
- POSTGRES_USER
- POSTGRES_PASS
- POSTGRES_DB
- POSTGRES_APP_USER
- POSTGRES_APP_PASS
- POSTGRES_DB_APP
- POSTGRES_DB_VCS
- REDIS_PASS
- JWT_ACCESS_SECRET
- JWT_REFRESH_SECRET

Then run plain Docker Compose deployment. It will catch .override.yml file automatically:

```sh
docker compose up -d --build
```

## Prod Deployment

Make sure you have all env variables set up in `.env`:

- INTERNAL_NUGET_HOST - Host for package registry to pull Mvcs.Core image from
- POSTGRES_USER
- POSTGRES_PASS
- POSTGRES_DB
- POSTGRES_APP_USER
- POSTGRES_APP_PASS
- POSTGRES_DB_APP
- POSTGRES_DB_VCS
- REDIS_PASS
- JWT_ACCESS_SECRET
- JWT_REFRESH_SECRET

Then run deployment with specified compose file:

```sh
docker compose -f docker-compose.yml up -d --build
```

If building with frontend included, make sure required version of mvcs-frontend image is available.
Then run deployment with specified compose files:

```sh
docker compose -f docker-compose.yml -f docker-compose.frontend.yml up -d --build
```
