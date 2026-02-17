using System;
using Microsoft.EntityFrameworkCore;
using SPRK.Backend.Entities;

namespace SPRK.Backend.Data{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){
            
        }

        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- 1. Konfigurasi Soft Delete Global ---
            // Setiap kali kita query (context.Rooms.ToList()), 
            // EF Core otomatis menambahkan "WHERE IsDeleted = false"
            modelBuilder.Entity<Room>().HasQueryFilter(r => !r.IsDeleted);
            modelBuilder.Entity<Booking>().HasQueryFilter(b => !b.IsDeleted);

            modelBuilder.Entity<Booking>()
                .Property(b => b.Status)
                .HasDefaultValue(BookingStatus.Pending);

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
                    CreatedAt = new DateTime(2026, 2, 14, 11, 24, 0, DateTimeKind.Utc),
                    IsDeleted = false
                },
                new Room
                {
                    Id = 2,
                    Name = "Ruang Theater Pascasarjana",
                    Capacity = 90,
                    Location = "Gedung Pascasarjana, Lantai 6",
                    Description = "Lengkap dengan proyektor, sound system, dan kursi empuk",
                    IsAvailable = true,
                    CreatedAt = new DateTime(2026, 2, 14, 11, 24, 0, DateTimeKind.Utc),
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
                    CreatedAt = new DateTime(2026, 2, 14, 11, 24, 0, DateTimeKind.Utc),
                    IsDeleted = false
                }
            );

            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    Id = 1,
                    RoomId = 1,
                    BorrowerName = "Andi Setiawan",
                    Purpose = "Seminar internal",
                    StartTime = new DateTime(2026, 2, 18, 2, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2026, 2, 18, 4, 0, 0, DateTimeKind.Utc),
                    Status = BookingStatus.Rejected,
                    CreatedAt = new DateTime(2026, 2, 14, 12, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false
                },
                new Booking
                {
                    Id = 2,
                    RoomId = 2,
                    BorrowerName = "Rina Kurnia",
                    Purpose = "Pelatihan lab",
                    StartTime = new DateTime(2026, 2, 19, 6, 30, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2026, 2, 19, 9, 0, 0, DateTimeKind.Utc),
                    Status = BookingStatus.Pending,
                    CreatedAt = new DateTime(2026, 2, 14, 12, 30, 0, DateTimeKind.Utc),
                    IsDeleted = false
                },
                new Booking
                {
                    Id = 3,
                    RoomId = 3,
                    BorrowerName = "Dosen TI",
                    Purpose = "Kuliah umum",
                    StartTime = new DateTime(2026, 2, 20, 1, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2026, 2, 20, 3, 0, 0, DateTimeKind.Utc),
                    Status = BookingStatus.Approved,
                    CreatedAt = new DateTime(2026, 2, 14, 13, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false
                },
                new Booking
                {
                    Id = 4,
                    RoomId = 1,
                    BorrowerName = "Mahasiswa TI",
                    Purpose = "Diskusi tugas akhir",
                    StartTime = new DateTime(2026, 2, 21, 10, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2026, 2, 21, 12, 0, 0, DateTimeKind.Utc),
                    Status = BookingStatus.Cancelled,
                    CreatedAt = new DateTime(2026, 2, 14, 14, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false
                }
            );
        }
    }
}