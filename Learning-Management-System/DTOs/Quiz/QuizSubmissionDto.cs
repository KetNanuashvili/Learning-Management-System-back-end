namespace Learning_Management_System.DTOs.Quiz
{
    public class QuizSubmissionDto
    {
        public int QuizId { get; set; }
        public Dictionary<int, string> Answers { get; set; } = new();
    }
}
