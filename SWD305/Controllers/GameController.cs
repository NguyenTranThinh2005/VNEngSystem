using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWD305.DTO;
using SWD305.Models;

namespace SWD305.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly VnegSystemContext _context;

        public GameController(VnegSystemContext context)
        {
            _context = context;
        }

        // =============================
        // START GAME
        // =============================
        [HttpPost("start")]
        public async Task<IActionResult> StartGame(StartGameRequest request)
        {
            var session = new GameSession
            {
                UserId = request.UserId,
                GameId = request.GameId,
                Score = 0,
                Stars = 0,
                Coins = 0
            };

            _context.GameSessions.Add(session);
            await _context.SaveChangesAsync();

            return Ok(new { sessionId = session.Id });
        }


        // =============================
        // GET QUESTIONS
        // =============================
        [HttpGet("{sessionId}/questions")]
        public async Task<IActionResult> GetQuestions(int sessionId)
        {
            var session = await _context.GameSessions.FindAsync(sessionId);
            if (session == null) return NotFound("Session not found");

            var questions = await _context.Questions
                .Where(q => q.GameId == session.GameId && q.IsActive == true)
                .Select(q => new
                {
                    q.Id,
                    q.Data,
                    q.QuestionType,
                    q.Difficulty
                })
                .ToListAsync();

            return Ok(questions);
        }

        // =============================
        // SUBMIT GAME (SERVER CHECK)
        // =============================
        [HttpPost("{sessionId}/submit")]
        public async Task<IActionResult> Submit(int sessionId, SubmitGameDto request)
        {
            var session = await _context.GameSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null) return NotFound("Session not found");

            int correctCount = 0;

            foreach (var item in request.Answers)
            {
                var question = await _context.Questions
                    .FirstOrDefaultAsync(q => q.Id == item.QuestionId);

                if (question == null) continue;

                var json = JsonDocument.Parse(question.Data);
                var answers = json.RootElement.GetProperty("answers");

                foreach (var answer in answers.EnumerateArray())
                {
                    int id = answer.GetProperty("id").GetInt32();
                    bool isCorrect = answer.GetProperty("isCorrect").GetBoolean();

                    if (id == item.SelectedAnswerId && isCorrect)
                    {
                        correctCount++;
                        break;
                    }
                }
            }

            int total = request.Answers.Count;

            session.Score = correctCount * 10;
            session.Accuracy = total == 0 ? 0 : (correctCount * 100m) / total;
            session.Stars = session.Accuracy >= 90 ? 3 :
                            session.Accuracy >= 70 ? 2 : 1;
            session.Coins = session.Stars * 5;
            session.CompletedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                session.Score,
                session.Accuracy,
                session.Stars,
                session.Coins
            });
        }

        // =============================
        // GET GAMES BY MAP
        // =============================
        [HttpGet("by-map/{mapId}")]
        public async Task<IActionResult> GetGamesByMap(int mapId)
        {
            var games = await _context.Games
                .Where(g => g.MapId == mapId)
                .Select(g => new
                {
                    g.Id,
                    g.Name,
                    g.GameType,
                    g.OrderIndex,
                    g.IsPremium
                })
                .ToListAsync();

            return Ok(games);
        }
    }

    // =============================
    // DTO
    // =============================
    public class SubmitGameDto
    {
        public List<UserAnswerDto> Answers { get; set; } = new();
    }

    public class UserAnswerDto
    {
        public int QuestionId { get; set; }
        public int SelectedAnswerId { get; set; }
    }
}
