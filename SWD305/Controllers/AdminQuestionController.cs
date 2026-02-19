using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWD305.DTO;
using SWD305.Models;

namespace SWD305.Controllers
{
    [ApiController]
    [Route("api/admin/questions")]
    public class AdminQuestionController : ControllerBase
    {
        private readonly VnegSystemContext _context;

        public AdminQuestionController(VnegSystemContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var questions = await _context.Questions
                .Include(q => q.Game)
                .ToListAsync();

            return Ok(questions);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            return Ok(question);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(CreateQuestionDto dto)
        {
            var question = new Question
            {
                GameId = dto.GameId,
                Data = dto.Data,
                QuestionType = dto.QuestionType,
                IsActive = dto.IsActive
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return Ok(question);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateQuestionDto dto)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            question.GameId = dto.GameId;
            question.Data = dto.Data;
            question.QuestionType = dto.QuestionType;
            question.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return Ok(question);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully");
        }
    }

}
