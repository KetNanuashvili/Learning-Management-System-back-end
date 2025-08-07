namespace Learning_Management_System.Models
{
    public class QuizQuestion
    {
        public int Id { get; set; }

        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;

        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;

       
        public string Options { get; set; } = string.Empty; // JSON string
        public string CorrectAnswer { get; set; } = string.Empty;
    }
}
