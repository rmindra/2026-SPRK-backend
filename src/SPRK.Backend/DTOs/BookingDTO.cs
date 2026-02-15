using System.ComponentModel.DataAnnotations;
using SPRK.Backend.Entities;

namespace SPRK.Backend.DTOs
{
    public record BookingCreateDTO(
        [Required] int RoomId,
        [Required, MaxLength(150)] string BorrowerName,
        [Required, MaxLength(500)] string Purpose,
        [Required] DateTime StartTime,
        [Required] DateTime EndTime
    );

    public record BookingUpdateDTO(
        [Required, MaxLength(150)] string BorrowerName,
        [Required, MaxLength(500)] string Purpose,
        [Required] DateTime StartTime,
        [Required] DateTime EndTime
    );

    public class BookingUpdateStatusDto
    {
        [Required]
        public BookingStatus Status { get; set; }
    }

    public class BookingFilterDto
    {
        public string? BorrowerName { get; set; }
        public int? RoomId { get; set; }
        public BookingStatus? Status { get; set; } // Menggunakan Enum agar type-safe
        public DateTime? Date { get; set; } // Filter per tanggal spesifik
        public string? SortBy { get; set; } // DateDesc (default), DateAsc, NameAsc, NameDesc
    }

    public class BookingResponseDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string BorrowerName { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}