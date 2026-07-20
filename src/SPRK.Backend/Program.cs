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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure database created dan jalankan migration pada saat startup
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    // Parse connection string untuk dapatkan host dan database name
    var connBuilder = new NpgsqlConnectionStringBuilder(dbConnectionString);
    var host     = connBuilder.Host;
    var port     = connBuilder.Port;
    var dbName   = connBuilder.Database ?? "SPRK";
    var username = connBuilder.Username;
    var password = connBuilder.Password;

    // Connect ke database default "postgres" untuk create DB jika belum ada
    var masterConnStr = $"Host={host};Port={port};Username={username};Password={password};Database=postgres";

    // Retry logic: PostgreSQL container mungkin belum ready saat pertama start
    int maxRetries = 10;
    int delayMs    = 2000;

    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            // Step 1: Buat database jika belum ada
            using (var masterConn = new NpgsqlConnection(masterConnStr))
            {
                masterConn.Open();
                var checkCmd = new NpgsqlCommand(
                    $"SELECT 1 FROM pg_database WHERE datname = '{dbName.ToLowerInvariant()}'",
                    masterConn);
                var exists = checkCmd.ExecuteScalar();

                if (exists == null)
                {
                    var createCmd = new NpgsqlCommand($"CREATE DATABASE \"{dbName}\"", masterConn);
                    createCmd.ExecuteNonQuery();
                    logger.LogInformation("Database {DatabaseName} created.", dbName);
                }
                else
                {
                    logger.LogInformation("Database {DatabaseName} already exists.", dbName);
                }
            }

            // Step 2: Jalankan EF Core migrations
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Attempt {Attempt}/{MaxRetries}: Failed to create/migrate database. Retrying in {Delay}ms...",
                i + 1, maxRetries, delayMs);

            if (i == maxRetries - 1)
            {
                logger.LogError(ex,
                    "Failed to create/migrate database after {MaxRetries} attempts. Application will continue but database may not be ready.",
                    maxRetries);
            }
            else
            {
                Thread.Sleep(delayMs);
            }
        }
    }
}


app.UseCors();

app.MapControllers();

app.Run();