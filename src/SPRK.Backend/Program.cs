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

// Swagger selalu aktif (berguna saat testing di Docker)
app.UseSwagger();
app.UseSwaggerUI();

// ─── Auto-create database dan jalankan migrations saat startup ───────────────
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

    var connBuilder = new NpgsqlConnectionStringBuilder(dbConnectionString);
    var host     = connBuilder.Host;
    var port     = connBuilder.Port;
    var dbName   = connBuilder.Database ?? "SPRK";
    var username = connBuilder.Username;
    var password = connBuilder.Password;

    // Connect ke database default "postgres" untuk ensure DB app exists
    var masterConnStr = $"Host={host};Port={port};Username={username};Password={password};Database=postgres";

    int maxRetries = 15;
    int delayMs    = 2000;
    Exception? lastEx = null;

    logger.LogInformation("Starting database initialization (max {MaxRetries} attempts)...", maxRetries);

    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            // Step 1: Pastikan database ada.
            // Langsung coba CREATE — jika sudah ada (42P04), tangkap dan lanjut.
            // Ini lebih robust daripada SELECT lalu CREATE (menghindari race condition & case sensitivity).
            using (var masterConn = new NpgsqlConnection(masterConnStr))
            {
                masterConn.Open();
                try
                {
                    using var createCmd = new NpgsqlCommand($"CREATE DATABASE \"{dbName}\"", masterConn);
                    createCmd.ExecuteNonQuery();
                    logger.LogInformation("✔ Database '{DatabaseName}' created.", dbName);
                }
                catch (PostgresException ex) when (ex.SqlState == "42P04")
                {
                    // 42P04 = duplicate_database — sudah ada, tidak masalah
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

    // Jika semua retry habis dan masih gagal → crash dengan jelas di docker logs
    if (lastEx != null)
    {
        logger.LogCritical(lastEx,
            "❌ FATAL: Could not initialize database after {MaxRetries} attempts. " +
            "Check PostgreSQL is running and connection string is correct. " +
            "ConnectionString: {ConnectionString}",
            maxRetries, dbConnectionString);
        throw new InvalidOperationException(
            $"Database initialization failed after {maxRetries} attempts.", lastEx);
    }
}
// ─────────────────────────────────────────────────────────────────────────────

app.UseCors();

app.MapControllers();

app.Run();