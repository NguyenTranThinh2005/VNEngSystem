using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWD305.DTO;
using SWD305.Models;

namespace SWD305.Controllers
{
    [ApiController]
    [Route("api/admin/games")]
    public class AdminGameController : ControllerBase
    {
        private readonly VnegSystemContext _context;

        public AdminGameController(VnegSystemContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var games = await _context.Games
                .Include(g => g.Map)
                .ToListAsync();

            return Ok(games);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var game = await _context.Games
                .Include(g => g.Map)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                return NotFound();

            return Ok(game);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(CreateGameDto dto)
        {
            var mapExists = await _context.Maps.AnyAsync(m => m.Id == dto.MapId);
            if (!mapExists)
                return BadRequest("MapId does not exist");

            var game = new Game
            {
                MapId = dto.MapId,
                Name = dto.Name,
                GameType = dto.GameType,
                Flow = dto.Flow,
                OrderIndex = dto.OrderIndex,
                IsPremium = dto.IsPremium
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return Ok(game);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateGameDto dto)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
                return NotFound();

            game.MapId = dto.MapId;
            game.Name = dto.Name;
            game.GameType = dto.GameType;
            game.Flow = dto.Flow;
            game.OrderIndex = dto.OrderIndex;
            game.IsPremium = dto.IsPremium;

            await _context.SaveChangesAsync();

            return Ok(game);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
                return NotFound();

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully");
        }
    }

}
