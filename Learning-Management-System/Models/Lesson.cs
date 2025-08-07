namespace Learning_Management_System.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}
