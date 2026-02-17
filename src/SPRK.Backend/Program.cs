using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.Data.SqlClient;
using SPRK.Backend.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Ambil Connection String dari appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Daftarkan DbContext ke Dependency Injection Container
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
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

// Auto-create database dan jalankan migration saat Development (termasuk di Docker)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Ensure database created dan jalankan migration
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        
        // Parse connection string untuk dapatkan server dan database name
    var builderConn = new SqlConnectionStringBuilder(dbConnectionString);
        var server = builderConn.DataSource;
        var databaseName = builderConn.InitialCatalog ?? "SPRK";
        var userId = builderConn.UserID;
        var password = builderConn.Password;
        
        // Buat connection string ke master database (tanpa specify database)
        var masterConnectionString = $"Server={server};User Id={userId};Password={password};TrustServerCertificate=True;";
        
        // Retry logic: SQL Server mungkin belum ready saat container pertama start
        int maxRetries = 10;
        int delayMs = 2000;
        bool databaseCreated = false;
        
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                // Step 1: Create database jika belum ada (connect ke master)
                using (var masterConn = new SqlConnection(masterConnectionString))
                {
                    masterConn.Open();
                    var createDbCommand = new SqlCommand(
                        $@"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{databaseName}')
                           BEGIN
                               CREATE DATABASE [{databaseName}];
                           END", masterConn);
                    createDbCommand.ExecuteNonQuery();
                    databaseCreated = true;
                    logger.LogInformation("Database {DatabaseName} ensured.", databaseName);
                }
                
                // Step 2: Migrate database (connect ke SPRK)
                if (databaseCreated)
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    dbContext.Database.Migrate();
                    logger.LogInformation("Database migrations applied successfully.");
                    break;
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Attempt {Attempt}/{MaxRetries}: Failed to create/migrate database. Retrying in {Delay}ms...", i + 1, maxRetries, delayMs);
                
                if (i == maxRetries - 1)
                {
                    logger.LogError(ex, "Failed to create/migrate database after {MaxRetries} attempts. Application will continue but database may not be ready.", maxRetries);
                }
                else
                {
                    Thread.Sleep(delayMs);
                }
            }
        }
    }
}

app.UseCors();

app.MapControllers();

app.Run();