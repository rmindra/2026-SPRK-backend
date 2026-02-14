using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPRK.Backend.Data;
using SPRK.Backend.DTOs;
using SPRK.Backend.Entities;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly AppDbContext _context;

    public RoomsController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomResponseDto>>> GetRooms()
    {
        var query = _context.Rooms.AsQueryable();

        var rooms = await query.ToListAsync();
        var result = rooms.Select(r => new RoomResponseDto
        {
            Id = r.Id,
            Name = r.Name,
            Capacity = r.Capacity,
            Location = r.Location,
            Description = r.Description,
            IsAvailable = r.IsAvailable,
            CreatedAt = r.CreatedAt
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomResponseDto>> GetRoom(int id)
    {
        var r = await _context.Rooms.FindAsync(id);
        if (r == null) return NotFound();

        return Ok(new RoomResponseDto
        {
            Id = r.Id,
            Name = r.Name,
            Capacity = r.Capacity,
            Location = r.Location,
            Description = r.Description,
            IsAvailable = r.IsAvailable,
            CreatedAt = r.CreatedAt
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RoomCreateDto dto)
    {
        var room = new Room
        {
            Name = dto.Name,
            Capacity = dto.Capacity,
            Location = dto.Location,
            Description = dto.Description,
            IsAvailable = dto.IsAvailable
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, new RoomResponseDto
        {
            Id = room.Id,
            Name = room.Name,
            Capacity = room.Capacity,
            Location = room.Location,
            Description = room.Description,
            IsAvailable = room.IsAvailable,
            CreatedAt = room.CreatedAt
        });
    }

    [HttpPut("{id}")]
public async Task<IActionResult> Update(int id, [FromBody] RoomUpdateDto dto)
{
    var room = await _context.Rooms.FindAsync(id);
    if (room == null) return NotFound(new { message = "Room tidak ditemukan" });

    room.Name = dto.Name;
    room.Capacity = dto.Capacity;
    room.Location = dto.Location;
    room.Description = dto.Description;
    room.IsAvailable = dto.IsAvailable;

    var affected = await _context.SaveChangesAsync();
    if (affected == 0) return BadRequest(new { message = "Tidak ada perubahan" });

    return Ok(new RoomResponseDto
    {
        Id = room.Id,
        Name = room.Name,
        Capacity = room.Capacity,
        Location = room.Location,
        Description = room.Description,
        IsAvailable = room.IsAvailable,
        CreatedAt = room.CreatedAt
    });
}

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var room = await _context.Rooms.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Id == id);
        if (room == null) return NotFound(new { message = "Room tidak ditemukan" });

        if (room.IsDeleted) return BadRequest(new { message = "Room sudah dihapus" });

        room.IsDeleted = true;

        var affected = await _context.SaveChangesAsync();
        if (affected == 0) return BadRequest(new { message = "Gagal menghapus room" });

        return Ok(new { message = "Room berhasil dihapus" });
    }
}