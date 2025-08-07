namespace Learning_Management_System.DTOs.User
{
    public class UserStatsDto
    {
        public int TotalCoursesEnrolled { get; set; }
        public int TotalLessonsCompleted { get; set; }
        public int QuizzesTaken { get; set; }
        public float AverageScore { get; set; }
    }
}
