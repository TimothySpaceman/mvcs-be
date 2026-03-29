FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY MVCS.Backend.sln .
COPY Api/Api.csproj Api/
COPY Shared/App.Infrastructure/App.Infrastructure.csproj Shared/App.Infrastructure/
COPY Modules/Users/Lib/Users.Lib.csproj Modules/Users/Lib/
RUN dotnet restore

COPY . .
RUN dotnet publish Api/Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Api.dll"]