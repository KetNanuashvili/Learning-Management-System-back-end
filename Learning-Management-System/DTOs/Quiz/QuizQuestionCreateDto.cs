namespace Learning_Management_System.DTOs.Quiz
{
    public class QuizQuestionCreateDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string OptionsJson { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
    }
}
