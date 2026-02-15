using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPRK.Backend.Data;
using SPRK.Backend.DTOs;
using SPRK.Backend.Entities;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _context;

    public BookingsController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetBookings()
    {
        var bookings = await _context.Bookings
        .Include(b => b.Room)
        .OrderBy(b => b.StartTime)
        .ToListAsync();

        var result = bookings.Select(b => new BookingResponseDto
        {
            Id = b.Id,
            RoomId = b.RoomId,
            RoomName = b.Room?.Name ?? string.Empty,
            BorrowerName = b.BorrowerName,
            Purpose = b.Purpose,
            StartTime = b.StartTime,
            EndTime = b.EndTime,
            Status = b.Status,
            CreatedAt = b.CreatedAt
        });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BookingCreateDTO dto)
    {
        if (dto.EndTime <= dto.StartTime)
            return BadRequest(new { message = "EndTime harus lebih besar dari StartTime" });

        var room = await _context.Rooms
            .Where(r => r.Id == dto.RoomId)
            .Select(r => new { r.Id, r.Name })
            .FirstOrDefaultAsync();

        if (room == null) return BadRequest(new { message = "RoomId tidak valid" });

        // cek bentrok
        var isOverlapping = await _context.Bookings.AnyAsync(b =>
            b.RoomId == dto.RoomId &&
            b.Status != BookingStatus.Rejected &&
            b.Status != BookingStatus.Cancelled &&
            dto.StartTime < b.EndTime &&
            dto.EndTime > b.StartTime);

        if (isOverlapping)
            return BadRequest(new { message = "Jadwal bentrok dengan booking lain" });

        var booking = new Booking
        {
            RoomId = dto.RoomId,
            BorrowerName = dto.BorrowerName,
            Purpose = dto.Purpose,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Status = BookingStatus.Pending
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, new BookingResponseDto
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            RoomName = room.Name,
            BorrowerName = booking.BorrowerName,
            Purpose = booking.Purpose,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Status = booking.Status,
            CreatedAt = booking.CreatedAt
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingResponseDto>> GetBooking(int id)
    {
        var b = await _context.Bookings
            .Include(x => x.Room)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (b == null) return NotFound(new { message = "Booking tidak ditemukan" });

        return Ok(new BookingResponseDto
        {
            Id = b.Id,
            RoomId = b.RoomId,
            RoomName = b.Room?.Name ?? string.Empty,
            BorrowerName = b.BorrowerName,
            Purpose = b.Purpose,
            StartTime = b.StartTime,
            EndTime = b.EndTime,
            Status = b.Status,
            CreatedAt = b.CreatedAt
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] BookingUpdateDTO dto)
    {
        if (dto.EndTime <= dto.StartTime)
            return BadRequest(new { message = "EndTime harus lebih besar dari StartTime" });

        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return NotFound(new { message = "Booking tidak ditemukan" });

        var isOverlapping = await _context.Bookings.AnyAsync(b =>
            b.Id != id &&
            b.RoomId == booking.RoomId &&
            b.Status != BookingStatus.Rejected &&
            b.Status != BookingStatus.Cancelled &&
            dto.StartTime < b.EndTime &&
            dto.EndTime > b.StartTime);

        if (isOverlapping)
            return BadRequest(new { message = "Jadwal bentrok dengan booking lain" });

        booking.BorrowerName = dto.BorrowerName;
        booking.Purpose = dto.Purpose;
        booking.StartTime = dto.StartTime;
        booking.EndTime = dto.EndTime;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Booking berhasil diupdate" });
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] BookingUpdateStatusDto dto)
    {
        // 1. Ambil Data Booking
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return NotFound("Data peminjaman tidak ditemukan.");

        // 2. Validasi Transisi Status (Tidak boleh ubah jika sudah final)
        if (booking.Status == BookingStatus.Cancelled || booking.Status == BookingStatus.Rejected)
        {
            return BadRequest("Tidak dapat mengubah status peminjaman yang sudah Dibatalkan atau Ditolak.");
        }

        // 3. Logic Check: Cek Konflik sebelum Approve
        if (dto.Status == BookingStatus.Approved)
        {
            bool isConflict = await _context.Bookings.AnyAsync(b =>
                b.Id != id &&                         // Abaikan booking diri sendiri
                b.RoomId == booking.RoomId &&         // Ruangan sama
                b.Status == BookingStatus.Approved && // Status lawan sudah Approved
                b.StartTime < booking.EndTime &&      // Cek Overlap Waktu
                b.EndTime > booking.StartTime
            );

            if (isConflict)
            {
                return Conflict("Gagal menyetujui: Sudah ada jadwal lain yang Disetujui (Approved) pada jam tersebut.");
            }
        }

        // 4. Simpan Perubahan
        booking.Status = dto.Status;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Status berhasil diperbarui" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var booking = await _context.Bookings.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.Id == id);
        if (booking == null) return NotFound(new { message = "Booking tidak ditemukan" });

        if (booking.IsDeleted) return BadRequest(new { message = "Booking sudah dihapus" });

        booking.IsDeleted = true;
        await _context.SaveChangesAsync();
        return Ok(new { message = "Booking berhasil dihapus (soft delete)" });
    }
}