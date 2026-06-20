FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

ARG INTERNAL_NUGET_HOST

COPY MVCS.Backend.sln .
COPY Api/Api.csproj Api/
COPY Lib/Lib.csproj Lib/
RUN if [ -n "INTERNAL_NUGET_HOST" ]; then dotnet nuget add source $INTERNAL_NUGET_HOST --name nuget_internal --allow-insecure-connections; fi
RUN dotnet restore

COPY . .
RUN dotnet publish Api/Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Api.dll"]