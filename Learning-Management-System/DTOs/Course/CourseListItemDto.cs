namespace Learning_Management_System.DTOs.Course
{
    public class CourseListItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
