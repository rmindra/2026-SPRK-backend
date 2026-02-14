using System.ComponentModel.DataAnnotations;

namespace SPRK.Backend.DTOs
{
    public class RoomCreateDto
    {
        [Required(ErrorMessage = "Nama ruangan wajib diisi")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 1000, ErrorMessage = "Kapasitas minimal 1 orang dan maksimal 1000 orang")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Lokasi ruangan wajib diisi")]
        [MaxLength(255)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsAvailable { get; set; } = true;
    }

    public class RoomUpdateDto : RoomCreateDto { }

    public class RoomResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}