namespace Learning_Management_System.DTOs.Statistics
{
    public class TopCoursesDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public int EnrollmentCount { get; set; }
    }
}
