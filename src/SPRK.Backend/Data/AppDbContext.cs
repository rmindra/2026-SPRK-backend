using System;
using Microsoft.EntityFrameworkCore;
using SPRK.Backend.Entities;

namespace SPRK.Backend.Data{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){
            
        }

        public DbSet<Room> Rooms { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            base.OnModelCreating(modelBuilder);

            // --- 1. Konfigurasi Soft Delete Global ---
            // Setiap kali kita query (context.Rooms.ToList()), 
            // EF Core otomatis menambahkan "WHERE IsDeleted = false"
            modelBuilder.Entity<Room>().HasQueryFilter(r => !r.IsDeleted);

            // --- 2. Data Seeding (Data Dummy Awal) ---
            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    Id = 1,
                    Name = "Ruang D3 Teater",
                    Capacity = 90,
                    Location = "Gedung D3, HH 101",
                    Description = "Lengkap dengan proyektor dan sound system",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Room
                {
                    Id = 2,
                    Name = "Ruang D4 Theater",
                    Capacity = 90,
                    Location = "Gedung Pascasarjana, Lantai 6",
                    Description = "Lengkap dengan proyektor, sound system, dan kursi empuk",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Room
                {
                    Id = 3,
                    Name = "Auditorium",
                    Capacity = 600,
                    Location = "Gedung Pascasarjana, Lantai 6",
                    Description = "Layar, Sound system, dan Ruangan super besar",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                }
            );
        }
    }
}