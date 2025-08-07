namespace Learning_Management_System.DTOs.Message
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public string SenderName { get; set; } = string.Empty;
    }
}
