namespace Learning_Management_System.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public int StudentId { get; set; }
        public ApplicationUser Student { get; set; } = null!;

        public float Progress { get; set; }
    }
}
