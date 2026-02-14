using Microsoft.EntityFrameworkCore;
using SPRK.Backend.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Ambil Connection String dari appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Daftarkan DbContext ke Dependency Injection Container
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();