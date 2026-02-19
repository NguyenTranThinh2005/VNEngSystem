namespace SWD305.DTO
{
    public class CreateQuestionDto
    {
        public int GameId { get; set; }
        public string Data { get; set; } = null!;
        public string QuestionType { get; set; } = null!;
        public int? Difficulty { get; set; }
        public string? Explanation { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UpdateQuestionDto
    {
        public int GameId { get; set; }
        public string Data { get; set; } = null!;
        public string QuestionType { get; set; } = null!;
        public int? Difficulty { get; set; }
        public string? Explanation { get; set; }
        public bool? IsActive { get; set; }
    }
}
