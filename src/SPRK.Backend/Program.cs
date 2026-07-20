using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Npgsql;
using SPRK.Backend.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Ambil Connection String dari config / environment variable
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Daftarkan DbContext dengan PostgreSQL provider
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

// Always expose Swagger (useful inside Docker for quick API testing)
app.UseSwagger();
app.UseSwaggerUI();

// ─── Auto-create database dan jalankan migrations saat startup ───────────────
// Retry logic: PostgreSQL container mungkin belum fully ready saat backend start.
// Jika semua retry habis dan masih gagal, aplikasi CRASH (terlihat jelas di docker logs).
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

    // Parse connection string untuk mendapatkan komponen host & database
    var connBuilder = new NpgsqlConnectionStringBuilder(dbConnectionString);
    var host     = connBuilder.Host;
    var port     = connBuilder.Port;
    var dbName   = connBuilder.Database ?? "SPRK";
    var username = connBuilder.Username;
    var password = connBuilder.Password;

    // Gunakan database default "postgres" untuk create app DB jika belum ada
    var masterConnStr = $"Host={host};Port={port};Username={username};Password={password};Database=postgres";

    int maxRetries = 15;
    int delayMs    = 2000;
    Exception? lastEx = null;

    logger.LogInformation("Starting database initialization (max {MaxRetries} attempts)...", maxRetries);

    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            // Step 1: Buat database jika belum ada
            using (var masterConn = new NpgsqlConnection(masterConnStr))
            {
                masterConn.Open();

                using var checkCmd = new NpgsqlCommand(
                    "SELECT 1 FROM pg_database WHERE datname = $1",
                    masterConn);
                checkCmd.Parameters.AddWithValue(dbName.ToLowerInvariant());
                var exists = checkCmd.ExecuteScalar();

                if (exists == null)
                {
                    // CREATE DATABASE tidak support parameterized query — nama DB sudah di-validate
                    using var createCmd = new NpgsqlCommand($"CREATE DATABASE \"{dbName}\"", masterConn);
                    createCmd.ExecuteNonQuery();
                    logger.LogInformation("✔ Database '{DatabaseName}' created.", dbName);
                }
                else
                {
                    logger.LogInformation("✔ Database '{DatabaseName}' already exists.", dbName);
                }
            }

            // Step 2: Jalankan semua pending EF Core migrations
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
            logger.LogInformation("✔ Database migrations applied successfully.");
            lastEx = null;
            break;
        }
        catch (Exception ex)
        {
            lastEx = ex;
            if (i < maxRetries - 1)
            {
                logger.LogWarning("  [{Attempt}/{MaxRetries}] DB not ready yet, retrying in {Delay}ms... ({Message})",
                    i + 1, maxRetries, delayMs, ex.Message);
                Thread.Sleep(delayMs);
            }
        }
    }

    // Jika semua retry habis dan masih gagal → crash dengan pesan yang jelas
    if (lastEx != null)
    {
        logger.LogCritical(lastEx,
            "❌ FATAL: Could not connect to or migrate the database after {MaxRetries} attempts. " +
            "Check that PostgreSQL is running and the connection string is correct. " +
            "Connection string: {ConnectionString}",
            maxRetries, dbConnectionString);
        throw new InvalidOperationException(
            $"Database initialization failed after {maxRetries} attempts. See logs above for details.",
            lastEx);
    }
}
// ─────────────────────────────────────────────────────────────────────────────

app.UseCors();

app.MapControllers();

app.Run();