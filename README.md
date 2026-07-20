# 2026-SPRK Backend

API backend untuk **Sistem Peminjaman Ruangan Kampus (SPRK)**, dibangun dengan **ASP.NET Core** menggunakan **Entity Framework Core** dan **PostgreSQL**.

---

## Tech Stack

| Layer         | Technology                                      |
|---------------|-------------------------------------------------|
| Framework     | ASP.NET Core (.NET 10)                          |
| ORM           | Entity Framework Core 10 (Code-First, Npgsql)   |
| Database      | PostgreSQL 16                                   |
| API Docs      | Swagger / OpenAPI (Swashbuckle)                 |
| Containerization | Docker (multi-stage build)                   |

---

## Project Structure

```
src/SPRK.Backend/
├── Controllers/
│   ├── RoomsController.cs     ← CRUD ruangan (GET, POST, PUT, DELETE)
│   └── BookingsController.cs  ← CRUD peminjaman + PATCH status + filter
├── DTOs/
│   ├── RoomDTO.cs             ← Request/response DTOs untuk ruangan
│   └── BookingDTO.cs          ← Request/response/filter DTOs untuk peminjaman
├── Entities/
│   ├── Room.cs                ← Entity Room (soft delete, audit fields)
│   └── Booking.cs             ← Entity Booking (status enum, soft delete)
├── Data/
│   └── AppDbContext.cs        ← DbContext, global query filter, data seeding
├── Migrations/                ← EF Core migration files (generated, PostgreSQL-specific)
├── Properties/
│   └── launchSettings.json    ← Local dev launch settings (port 5006)
├── appsettings.json           ← Base config (connection string, CORS, logging)
├── appsettings.Development.json ← Dev overrides (verbose logging, local DB)
└── Program.cs                 ← App bootstrap, DI, CORS, auto-migrate on startup
```

---

## API Endpoints

### Rooms — `/api/Rooms`

| Method | Route             | Description                          |
|--------|-------------------|--------------------------------------|
| GET    | `/api/Rooms`      | Get all rooms (soft-delete filtered) |
| GET    | `/api/Rooms/{id}` | Get room by ID                       |
| POST   | `/api/Rooms`      | Create new room                      |
| PUT    | `/api/Rooms/{id}` | Update room                          |
| DELETE | `/api/Rooms/{id}` | Soft-delete room                     |

### Bookings — `/api/Bookings`

| Method | Route                       | Description                                     |
|--------|-----------------------------|--------------------------------------------------|
| GET    | `/api/Bookings`             | Get bookings (supports filter + sort via query)  |
| GET    | `/api/Bookings/{id}`        | Get booking by ID                                |
| POST   | `/api/Bookings`             | Create new booking (overlap check)               |
| PUT    | `/api/Bookings/{id}`        | Update booking details (overlap check)           |
| PATCH  | `/api/Bookings/{id}/status` | Update booking status                            |
| DELETE | `/api/Bookings/{id}`        | Soft-delete booking                              |

#### Booking Filter Query Params (`GET /api/Bookings`)

| Param          | Type   | Description                                           |
|----------------|--------|-------------------------------------------------------|
| `borrowerName` | string | Filter by partial borrower name                       |
| `roomId`       | int    | Filter by room ID                                     |
| `status`       | string | `Pending`, `Approved`, `Rejected`, `Cancelled`        |
| `date`         | date   | Filter by specific date (matches `startTime`)         |
| `sortBy`       | string | `DateDesc` (default), `DateAsc`, `NameAsc`, `NameDesc`|

---

## Running Locally (without Docker)

### Prerequisites

- .NET 10 SDK
- PostgreSQL 16 running locally

  Quick way to run PostgreSQL via Docker:
  ```bash
  docker run -d --name sprk-pg \
    -e POSTGRES_PASSWORD=YourStrong!Passw0rd \
    -p 5432:5432 \
    postgres:16-alpine
  ```

### Setup

```bash
# 1. Navigate to the project
cd src/SPRK.Backend

# 2. Review / edit appsettings.Development.json
#    Update the connection string if your PostgreSQL uses different credentials.

# 3. Run (auto-creates DB and applies migrations on startup)
dotnet run
```

API runs at **http://localhost:5006**  
Swagger UI available at **http://localhost:5006/swagger**

> **Note:** The initial PostgreSQL migrations (`Migrations/`) are included in the repo.
> When you start the application (`dotnet run` or `make dev`), it automatically checks, creates the database if missing, and applies all pending migrations.

---

## Running via Docker (recommended)

Use the infrastructure repo to launch everything together:

```bash
# From 2026-SPRK-infrastructure/
make dev
```

See [`2026-SPRK-infrastructure/README.md`](../2026-SPRK-infrastructure/README.md) for full instructions.

---

## Environment Variables

Set via `appsettings.json` / `appsettings.Development.json`, or overridden at runtime by Docker Compose:

| Variable                               | Default (local)                                                    | Description                  |
|----------------------------------------|--------------------------------------------------------------------|------------------------------|
| `ConnectionStrings__DefaultConnection` | `Host=localhost;Port=5432;Database=SPRK;Username=postgres;Password=...` | PostgreSQL connection string |
| `ASPNETCORE_ENVIRONMENT`               | `Development`                                                      | Environment name             |
| `ASPNETCORE_URLS`                      | `http://localhost:5006`                                            | Bind address & port          |
| `Cors__AllowedOrigins__0`              | `http://localhost:5173`                                            | First allowed CORS origin    |

---

## Key Design Decisions

- **PostgreSQL via Npgsql**: Uses `Npgsql.EntityFrameworkCore.PostgreSQL` provider — cross-platform, no Windows dependency.
- **Soft Delete**: `Room` and `Booking` have an `IsDeleted` flag. A global EF query filter automatically excludes deleted records.
- **Auto-Migration on Startup**: `Program.cs` always runs EF Core migrations on every startup regardless of environment. It attempts to create the database (safely ignores if already exists) then applies all pending migrations, with retry logic for Docker cold-starts.
- **Overlap Detection**: `POST` and `PUT` bookings check for time overlap with existing non-rejected/cancelled bookings.
- **Conflict Check on Approve**: `PATCH /status` checks for approved booking conflicts before allowing `Approved`.

---

## License

UNLICENSED (internal project).
