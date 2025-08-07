namespace Learning_Management_System.DTOs.Quiz
{
    public class QuizCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public int CourseId { get; set; }
    }
}
