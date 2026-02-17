# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj dan restore
COPY src/SPRK.Backend/SPRK.Backend.csproj src/SPRK.Backend/
RUN dotnet restore src/SPRK.Backend/SPRK.Backend.csproj

# Copy semua source lalu publish (restore lagi untuk memastikan semua dependency ter-resolve)
COPY . .
WORKDIR /src/src/SPRK.Backend
RUN dotnet publish -c Release -o /app/publish

# Run stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Connection string & URL di-set via env dari docker-compose
EXPOSE 5006
ENTRYPOINT ["dotnet", "SPRK.Backend.dll"]
