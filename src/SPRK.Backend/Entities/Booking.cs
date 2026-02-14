using System.ComponentModel.DataAnnotations;

namespace SPRK.Backend.Entities
{
    public enum BookingStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled
    }

    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public int RoomId { get; set; }
        public Room? Room { get; set; }

        [Required]
        [MaxLength(150)]
        public string BorrowerName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Purpose { get; set; } = string.Empty;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;
    }
}