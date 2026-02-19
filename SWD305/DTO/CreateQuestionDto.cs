namespace SWD305.DTO
{
    public class CreateQuestionDto
    {
        public int GameId { get; set; }
        public string Data { get; set; }
        public string QuestionType { get; set; }
        public bool IsActive { get; set; }
    }

}
