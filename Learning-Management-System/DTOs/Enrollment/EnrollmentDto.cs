namespace Learning_Management_System.DTOs.Enrollment
{
    public class EnrollmentDto
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public float Progress { get; set; }
    }
}
