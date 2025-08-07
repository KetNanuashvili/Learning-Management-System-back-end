namespace Learning_Management_System.DTOs.Message
{
    public class SendMessageDto
    {
        
        public int CourseId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
