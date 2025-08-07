namespace Learning_Management_System.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public int SenderId { get; set; }
        public ApplicationUser Sender { get; set; } = null!;

        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
}

