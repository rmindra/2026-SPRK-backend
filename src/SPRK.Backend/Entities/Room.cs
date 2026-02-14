using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPRK.Backend.Entities
{
    [Table("Rooms")]
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama ruangan wajib diisi")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 1000, ErrorMessage = "Kapasitas minimal 1 orang dan maksimal 1000 orang")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Lokasi ruangan wajib diisi")]
        [MaxLength(255)]
        public string Location { get; set; } = string.Empty;

        public string? Description { get; set; } // Boleh null (opsional)

        public bool IsAvailable { get; set; } = true;

        // --- Audit & Soft Delete ---

        // Menandakan kapan data dibuat
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Flag untuk Soft Delete. Jika true, dianggap terhapus.
        public bool IsDeleted { get; set; } = false;
    }
}