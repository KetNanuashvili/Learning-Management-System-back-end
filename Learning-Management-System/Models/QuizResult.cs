namespace Learning_Management_System.Models
{
    public class QuizResult
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;

        public int StudentId { get; set; }
        public ApplicationUser Student { get; set; } = null!;

        public float Score { get; set; }
    }
}
