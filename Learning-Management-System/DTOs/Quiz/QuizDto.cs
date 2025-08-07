namespace Learning_Management_System.DTOs.Quiz
{
    public class QuizDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<QuizQuestionCreateDto> Questions { get; set; } = new();
    }
}
